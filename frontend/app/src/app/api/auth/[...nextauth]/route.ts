import NextAuth from "next-auth"
import CredentialsProvider from "next-auth/providers/credentials"
import bcrypt from "bcryptjs"
import { users } from "@/lib/db"

export const { handlers, signIn, signOut, auth } = NextAuth({
  providers: [
    CredentialsProvider({
      name: "Credentials",
      credentials: {
        email: { label: "Email", type: "email" },
        password: { label: "Password", type: "password" },
      },
      async authorize(credentials) {
        if (!credentials || typeof credentials.email !== "string" || typeof credentials.password !== "string") {
          return null
        }

        const { email, password } = credentials

        const user = users.find((u) => u.email === email)

        if (!user) {
          return null
        }

        // Verificar contraseña con bcrypt
        try {
          const isValid = await bcrypt.compare(password, user.password)
          if (isValid) {
            return {
              id: user.id,
              email: user.email,
              name: user.name,
            }
          }
        } catch (error) {
          // Si el hash no es válido (usuario demo), verificar directamente
          if (password === "demo123") {
            return {
              id: user.id,
              email: user.email,
              name: user.name,
            }
          }
        }

        return null
      },
    }),
  ],
  pages: {
    signIn: "/login",
    signOut: "/login",
  },
  session: {
    strategy: "jwt",
  },
  secret: process.env.NEXTAUTH_SECRET || "fallback-secret-key-change-in-production",
  trustHost: true,
  callbacks: {
    async jwt({ token, user }) {
      if (user) {
        token.id = user.id
        token.email = user.email
        token.name = user.name
      }
      return token
    },
    async session({ session, token }) {
      if (session.user) {
        session.user.id = token.id as string
        session.user.email = token.email as string
        session.user.name = token.name as string
      }
      return session
    },
  },
})

export const { GET, POST } = handlers

