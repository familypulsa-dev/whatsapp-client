import { create } from "zustand"
import type { WebsocketEvent } from "../types/wsEvent"

interface WSState {
  connected: boolean
  connect: (token: string, company_id: string) => void
  disconnect: () => void
  onEvent: ((ev: WebsocketEvent) => void) | null
  sendEvent: (event: string, payload: any, token?: string) => void
}

export const useWS = create<WSState>((set, get) => {
  let ws: WebSocket | null = null
  let currentToken: string | null = null
  let currentCompanyId: string | null = null
  
  let isRefreshing = false
  let offlineQueue: { event: string, payload: any }[] = []
  let pendingRequests = new Map<string, { event: string, payload: any }>()

  function connect(token: string, company_id: string) {
    if (ws) {
      ws.onclose = null
      ws.onerror = null
      ws.onmessage = null
      ws.close()
    }
    currentToken = token
    currentCompanyId = company_id
    const host = (window as any).__WS_HOST__ || "localhost:8080"
    // const host = (window as any).__WS_HOST__ || "test.waba.mbi-software.com"
    const proto = host.startsWith("localhost") ? "ws:" : "wss:"
    
    // Connect tanpa query parameter
    ws = new WebSocket(`${proto}//${host}/ws/`)

    ws.onopen = () => {
      set({ connected: true })
      
      const safeCompanyId = company_id || '00000000-0000-0000-0000-000000000000'

      // Kirim payload auth_company sesaat setelah terkoneksi
      ws?.send(JSON.stringify({
        id: "1",
        event: "auth_company",
        payload: {
          company_id: safeCompanyId,
          access_token: token
        }
      }))

      // Buka gembok antrean (berlaku jika baru saja sukses rebirth/refresh)
      isRefreshing = false

      // Muntahkan semua antrean pesan yang sempat tertunda
      while (offlineQueue.length > 0) {
        const req = offlineQueue.shift()
        if (req) {
          sendEvent(req.event, req.payload)
        }
      }
    }
    
    ws.onclose = () => set({ connected: false })
    ws.onerror = () => set({ connected: false })

    ws.onmessage = (msg) => {
      try {
        const ev = JSON.parse(msg.data)
        console.log("WS Received:", ev)

        // 1. Deteksi token expired/unauthorized
        const errMsg = ev.error ? (typeof ev.error === 'object' ? ev.error.message : ev.error) : "";
        if (errMsg && (errMsg.includes("unauthorized") || errMsg.includes("expired") || ev.error?.code === 401)) {
          const wasRefreshing = isRefreshing;
          isRefreshing = true
          
          const failedReq = ev.id ? pendingRequests.get(ev.id) : Array.from(pendingRequests.values()).pop()
          if (failedReq) {
            offlineQueue.push(failedReq)
            pendingRequests.clear()
          }

          const refreshToken = localStorage.getItem("refresh_token")
          
          // Cegah spam event auth_refresh jika sedang dalam proses refresh
          if (!wasRefreshing) {
            if (refreshToken && ws && ws.readyState === WebSocket.OPEN) {
              console.log("WS Token expired, triggering auth_refresh...")
              ws.send(JSON.stringify({
                id: crypto.randomUUID(),
                event: "auth_refresh",
                payload: {
                  refresh_token: refreshToken
                }
              }))
            } else {
               // Jika tidak ada refresh token, biarkan user logout atau lempar ke UI
               get().onEvent?.(ev as WebsocketEvent)
            }
          }
          return // Hentikan propagasi event error ke UI
        }

        // 2. Deteksi sukses refresh token
        if ((ev.event === "auth_refresh" || ev.event === "auth_success" || ev.event === "auth_refresh_success") && !ev.error) {
          const newToken = ev.payload?.access_token || ev.access_token
          if (newToken) {
            currentToken = newToken
            // isRefreshing jangan di-false di sini, biarkan onopen yang membuka setelah reborn
            localStorage.setItem("token", newToken)
            
            const newRefreshToken = ev.payload?.refresh_token || ev.refresh_token
            if (newRefreshToken) {
              localStorage.setItem("refresh_token", newRefreshToken)
            }
            console.log("WS Token refreshed successfully! Initiating Rebirth...")

            // 🔥 FORCE RECONNECT (BUNUH DIRI LALU KONEK ULANG) 🔥
            // Memutus koneksi lama agar backend mereset flag FilterByPhone
            // connect() otomatis melakukan ws.close() dan new WebSocket()
            const savedCompanyId = currentCompanyId || '00000000-0000-0000-0000-000000000000'
            setTimeout(() => connect(newToken, savedCompanyId), 100)
          }
          return // Refresh success handler selesai
        }

        get().onEvent?.(ev as WebsocketEvent)
      } catch {
        /* ignore malformed */
      }
    }
  }

  function disconnect() {
    ws?.close()
    ws = null
    currentToken = null
    currentCompanyId = null
    set({ connected: false })
  }

  function sendEvent(eventName: string, payload: any, tokenOverride?: string) {
    if (isRefreshing) {
      offlineQueue.push({ event: eventName, payload: payload })
      return
    }

    if (ws && ws.readyState === WebSocket.OPEN) {
      const activeToken = tokenOverride || currentToken || ""
      const reqId = crypto.randomUUID()
      
      pendingRequests.set(reqId, { event: eventName, payload: payload })

      ws.send(JSON.stringify({
        id: reqId,
        event: eventName,
        access_token: activeToken, // Di root (sesuai spesifikasi)
        payload: {
          ...payload,
          access_token: activeToken // Di dalam payload (jaga-jaga seperti auth_company)
        }
      }))
      
      // Hapus dari memori otomatis setelah 10 detik agar tidak bocor
      setTimeout(() => pendingRequests.delete(reqId), 10000)
    } else {
      console.warn("WebSocket belum terkoneksi! Gagal mengirim event:", eventName)
    }
  }

  return { connected: false, connect, disconnect, onEvent: null, sendEvent }
})
