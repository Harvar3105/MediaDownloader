export const locales = ['en', 'ru'] as const

export type Locale = typeof locales[number]

export type Translations = {
  pageTitle: string
  downloadMedia: string
  description: string
  mediaLink: string
  fetchStreams: string
  fetchingStreams: string
  selectFormat: string
  audio: string
  video: string
  prepareFile: string
  preparingFile: string
  downloadFile: string
  fetchStreamsError: string
  noStreamsError: string
  prepareFileError: string
  requestError: string
  language: string
}

export async function loadTranslations(locale: Locale) {
  const response = await fetch(`${import.meta.env.BASE_URL}locales/${locale}.json`)

  if (!response.ok) {
    throw new Error(`Unable to load ${locale} translations.`)
  }

  return response.json() as Promise<Translations>
}
