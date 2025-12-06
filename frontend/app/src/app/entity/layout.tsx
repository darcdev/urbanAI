"use client";

import { Navbar } from "./components";

export default function EntityLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <div className="flex h-screen w-full overflow-hidden bg-background flex-col">
      {/* Navbar */}
      <Navbar />
      
      {/* Page Content - Sin padding para que el mapa ocupe todo */}
      <main className="flex-1 overflow-hidden">
        {children}
      </main>
    </div>
  );
}