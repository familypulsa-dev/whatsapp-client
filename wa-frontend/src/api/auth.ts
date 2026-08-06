import { post, get } from "./client"
import type { User } from "../types"

export interface LoginResponse {
  access_token: string
  refresh_token: string
  user: User
}

export interface Meta<T>{
  data ?: T
  meta ?: any
  error ?: {
    message: string
  }
}

export function login(username: string, password: string): Promise<Meta<LoginResponse>> {
  return post<Meta<LoginResponse>>("/api/v1/auth/login", { username, password })
}

export function getMe(): Promise<Meta<User>> {
  return get<Meta<User>>("/api/v1/auth/me")
}
