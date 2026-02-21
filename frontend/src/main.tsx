import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'

import "./index.css"
import Home from './pages/Home'
import { AuthProvider } from './hooks/useUser'

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <AuthProvider>
      <Home />
    </AuthProvider>
  </StrictMode>,
)
