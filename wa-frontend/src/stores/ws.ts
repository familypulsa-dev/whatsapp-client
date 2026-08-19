import { create } from "zustand"
import type { Conversation, WSEvent } from "../types"

interface WSState {
  connected: boolean
  connect: (token: string, company_id: string) => void
  disconnect: () => void
  onEvent: ((ev: WSEvent) => void) | null
}

export const useWS = create<WSState>((set, get) => {
  let ws: WebSocket | null = null

  function connect(token: string, company_id: string) {
    if (ws) ws.close()
    const host = (window as any).__WS_HOST__ || "localhost:8080"
    const proto = host.startsWith("localhost") ? "ws:" : "wss:"
    
    // Connect tanpa query parameter
    ws = new WebSocket(`${proto}//${host}/ws`)

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
    }
    
    ws.onclose = () => set({ connected: false })
    ws.onerror = () => set({ connected: false })

    ws.onmessage = (msg) => {
      try {
        const ev: WSEvent = JSON.parse(msg.data)
        console.log(ev)
        get().onEvent?.(ev)
      } catch {
        /* ignore malformed */
      }
    }
  }

  function disconnect() {
    ws?.close()
    ws = null
    set({ connected: false })
  }

  return { connected: false, connect, disconnect, onEvent: null }
})
