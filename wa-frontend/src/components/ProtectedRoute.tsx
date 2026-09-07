import { useEffect } from "react"
import { useNavigate } from "react-router-dom"
import { useAuth } from "../stores/auth"

export default function ProtectedRoute({ children }: { children: React.ReactNode }) {
  const navigate = useNavigate()
  const { token, user, loading, init } = useAuth()

  useEffect(() => {
    init()
  }, [init])

  useEffect(() => {
    if (loading) return
    if (!token) {
      navigate("/login", { replace: true })
      return
    }
  }, [token, loading, navigate])

  if (loading) {
    return (
      <div className="flex h-screen items-center justify-center">
        <div className="h-8 w-8 animate-spin rounded-full border-4 border-gray-300 border-t-blue-600" />
      </div>
    )
  }

  if (!token || !user) return null

  return <>{children}</>
}
