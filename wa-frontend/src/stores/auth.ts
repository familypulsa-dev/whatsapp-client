import { create } from "zustand"
import type { User } from "../types"
import * as authApi from "../api/auth"
import { useWS } from "./ws"

interface AuthState {
  token: string | null
  user: User | null
  loading: boolean
  setAuth: (token: string, refreshToken: string, user: User) => void
  logout: () => void
  init: () => Promise<void>
}

export const useAuth = create<AuthState>((set) => ({
  token: localStorage.getItem("token"),
  user: null,
  loading: true,

  setAuth: (token, refreshToken, user) => {
    localStorage.setItem("token", token)
    localStorage.setItem("refresh_token", refreshToken)
    set({ token, user, loading: false })
  },

  logout: () => {
    localStorage.removeItem("token")
    localStorage.removeItem("refresh_token")
    useWS.getState().disconnect()
    set({ token: null, user: null, loading: false })
  },

  init: async () => {
    const token = localStorage.getItem("token")

    if (!token) {
      set({ token: null, user: null, loading: false })
      return
    }

    try {
      const response = await authApi.getMe()
      if (response.error || !response.data) {
        throw new Error(response.error?.message || "Failed to get user data")
      }
      const activeToken = localStorage.getItem("token") || token
      set({ token: activeToken, user: response.data, loading: false })
    } catch {
      localStorage.removeItem("token")
      localStorage.removeItem("refresh_token")
      useWS.getState().disconnect()
      set({ token: null, user: null, loading: false })
    }
  },
}))
