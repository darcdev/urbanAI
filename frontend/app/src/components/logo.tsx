import { cn } from "@/lib/utils"
import Image from "next/image";

interface LogoProps {
  className?: string
  size?: "sm" | "md" | "lg" | "xl"
  showIcon?: boolean
}

const sizeClasses = {
  sm: "text-lg",
  md: "text-2xl",
  lg: "text-3xl",
  xl: "text-4xl",
}

const iconSizeClasses = {
  sm: "w-4 h-4",
  md: "w-5 h-5",
  lg: "w-6 h-6",
  xl: "w-7 h-7",
}

export function Logo({ className, size = "md", showIcon = true }: LogoProps) {
  return (
    <span className={cn("font-bold inline-flex items-center gap-2", sizeClasses[size], className)}>
      {showIcon && (
        <Image
          src="/images/Logo-liviano.png"
          alt="UrbanAI"
          width={120}
          height={40}
          className="object-contain"
        />
      )}
    </span>
  )
}

