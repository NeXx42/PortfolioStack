import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'

import "./index.css"
import Home from './pages/Home'
import { AuthProvider } from './hooks/useUser'
import { BrowserRouter, Route, Routes } from 'react-router-dom'
import NotFound from './pages/NotFound'
import Content from './pages/Content'
import Admin from './pages/Admin'

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <AuthProvider>

      <BrowserRouter>
        <Routes>
          <Route path="" element={<Home />} />
          <Route path="/:slug/content" element={<Content />} />
          <Route path="/admin" element={<Admin />} />

          <Route path="*" element={<NotFound />} />

        </Routes>
      </BrowserRouter>

    </AuthProvider>
  </StrictMode>,
)
