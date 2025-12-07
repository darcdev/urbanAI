import { Logo } from "@/components/logo"
import { Button } from "@/components/ui/button"

interface HeaderBarProps {
  onOpenModal: () => void
}

export function HeaderBar({ onOpenModal }: HeaderBarProps) {
  return (
    <div className="absolute top-0 left-0 w-full z-50 bg-white/85 backdrop-blur-xl border-b border-border/40 shadow-sm">
      <div className="px-4 py-4 max-w-5xl mx-auto flex items-center justify-between">
        <Logo size="md" />
        <Button onClick={onOpenModal} className="rounded-md h-10 px-4 font-semibold">
          Reportar incidente
        </Button>
      </div>
    </div>
  )
}


