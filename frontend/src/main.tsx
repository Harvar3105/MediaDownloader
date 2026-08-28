import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import Home from './home'
import { loadTranslations } from './locales'

async function renderApp() {
  const initialTranslations = await loadTranslations('en')

  createRoot(document.getElementById('root')!).render(
    <StrictMode>
      <Home initialTranslations={initialTranslations} />
    </StrictMode>,
  )
}

void renderApp()
