import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  output: "standalone",
  images: {
    remotePatterns: [
      {
        protocol: "http",
        hostname: "localhost",
        port: "9000",
        pathname: "/incident-images/**",
      },
      {
        protocol: "https",
        hostname: "www.mintic.gov.co",
        pathname: "/portal/**",
      },
    ],
  },
};

export default nextConfig;
