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
      const host = (window as any).__WS_HOST__ || "localhost:8081"
    const proto = host.startsWith("localhost") ? "ws:" : "wss:"
    company_id = company_id == undefined || company_id == null ? '00000000-0000-0000-0000-000000000000' : company_id
    ws = new WebSocket(`${proto}//${host}/ws?company_id=${company_id}&token=${token}`)

    ws.onopen = () => set({ connected: true })
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
