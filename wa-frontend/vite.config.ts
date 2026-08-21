import tailwindcss from "@tailwindcss/vite"
import react from "@vitejs/plugin-react"
import { defineConfig } from "vite"
import path from "path"
import { config } from 'dotenv';

// Load environment variables from .env file
config();

export default defineConfig({
  plugins: [react(), tailwindcss()],
  resolve: {
    alias: {
      "@": path.resolve(__dirname, "./src"),
    },
  },
  build: {
    rollupOptions: {
      output: {
        entryFileNames: 'assets/index.js',
        chunkFileNames: 'assets/[name].js',
        assetFileNames: 'assets/[name][extname]',
      },
    },
  },
  server: {
    port: 5173,
    proxy: {
      "/api": "https://test.waba.mbi-software.com",
      "/ws": {
        target: "wss://test.waba.mbi-software.com",
        ws: true,
      },
    },
  },  
   // Your Vite configuration
  define: {
    'process.env': process.env
  }
})
