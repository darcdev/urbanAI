import { auth } from "@/app/api/auth/[...nextauth]/route"
import { NextResponse } from "next/server"

export async function middleware() {
  //const session = await auth()
  /*
  if (!session) {
    return NextResponse.redirect(new URL("/login", process.env.NEXTAUTH_URL || "http://localhost:3000"))
  }
  */
  return NextResponse.next()
}

export const config = {
  matcher: ["/dashboard/:path*"],
}

