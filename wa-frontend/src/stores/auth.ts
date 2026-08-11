import { create } from "zustand"
import type { User } from "../types"
import { useWS } from "./ws"

interface AuthState {
  token: string | null
  user: User | null
  loading: boolean
  setAuth: (token: string, refreshToken: string, user: User) => void
  logout: () => void
  init: () => Promise<void>
}

export const useAuth = create<AuthState>((set) => {
  const storedUser = localStorage.getItem("user")
  const initialUser = storedUser ? JSON.parse(storedUser) : null

  return {
    token: localStorage.getItem("access_token"),
    user: initialUser,
    loading: !localStorage.getItem("access_token"),

    setAuth: (token, refreshToken, user) => {
      console.log(`[setAuth] token: ${token}, refreshToken: ${refreshToken}, user: ${JSON.stringify(user)}`)
      localStorage.setItem("access_token", token)
      localStorage.setItem("refresh_token", refreshToken)
      localStorage.setItem("user", JSON.stringify(user))
      
      useWS.getState().connect(token, user.company_id || "")
      set({ token, user, loading: false })
    },

    logout: () => {
      localStorage.removeItem("access_token")
      localStorage.removeItem("refresh_token")
      localStorage.removeItem("user")
      
      useWS.getState().disconnect()
      set({ token: null, user: null, loading: false })
    },

    init: async () => {
      const token = localStorage.getItem("access_token")
      const storedUser = localStorage.getItem("user")
      
      if (!token || !storedUser) {
        localStorage.removeItem("access_token")
        localStorage.removeItem("refresh_token")
        localStorage.removeItem("user")
        set({ loading: false })
        return
      }
      
      try {
        const user = JSON.parse(storedUser)
        useWS.getState().connect(token, user.company_id || "")
        set({ token, user, loading: false })
      } catch {
        localStorage.removeItem("access_token")
        localStorage.removeItem("refresh_token")
        localStorage.removeItem("user")
        set({ token: null, user: null, loading: false })
      }
    },
  }
})
