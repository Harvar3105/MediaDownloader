import { useEffect, useState, type FormEvent } from 'react'
import { loadTranslations, type Locale, type Translations } from './locales'

type StreamInfo = {
  id: string
  videoExtension: string
  resolution: string
  audioCodec: string
  videoCodec: string
  totalBitrate: number | null
  videoBitrate: number | null
  audioBitrate: number | null
}

const apiUrl = `http://localhost:${import.meta.env.DEV ? '5003' : '5000'}`

function getFileName(contentDisposition: string | null, fallback: string) {
  const match = contentDisposition?.match(/filename\*?=(?:UTF-8'')?["']?([^;"']+)/i)
  return match ? decodeURIComponent(match[1]) : fallback
}

function isAudioStream(stream: StreamInfo) {
  return stream.videoCodec.toLowerCase().includes('audio only') || stream.resolution.toLowerCase().includes('audio only')
}

function getResolution(stream: StreamInfo) {
  const match = stream.resolution.match(/(?:^|\D)(144|240|360|480|720|1080|1440|2160)p?(?:\D|$)/i)
  return match?.[1] ?? '720'
}

function getBitrateLabel(value: number | null) {
  return value === null ? null : `${value} kbps`
}

type HomeProps = {
  initialTranslations: Translations
}

export default function Home({ initialTranslations }: HomeProps) {
  const [locale, setLocale] = useState<Locale>('en')
  const [t, setTranslations] = useState(initialTranslations)
  const [link, setLink] = useState('')
  const [streams, setStreams] = useState<StreamInfo[]>([])
  const [selectedStream, setSelectedStream] = useState<StreamInfo | null>(null)
  const [downloadUrl, setDownloadUrl] = useState<string | null>(null)
  const [fileName, setFileName] = useState('media-download')
  const [error, setError] = useState<string | null>(null)
  const [isFetchingStreams, setIsFetchingStreams] = useState(false)
  const [isPreparingFile, setIsPreparingFile] = useState(false)
  useEffect(() => () => {
    if (downloadUrl) URL.revokeObjectURL(downloadUrl)
  }, [downloadUrl])

  useEffect(() => {
    document.documentElement.lang = locale
    document.title = t.pageTitle
  }, [locale, t.pageTitle])

  useEffect(() => {
    if (locale === 'en') return

    let isCurrent = true

    loadTranslations(locale).then((translations) => {
      if (isCurrent) setTranslations(translations)
    })

    return () => {
      isCurrent = false
    }
  }, [locale])

  function clearDownload() {
    if (downloadUrl) URL.revokeObjectURL(downloadUrl)
    setDownloadUrl(null)
  }

  function handleLinkChange(value: string) {
    setLink(value)
    setStreams([])
    setSelectedStream(null)
    setError(null)
    clearDownload()
  }

  async function handleFetchStreams(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setError(null)
    setIsFetchingStreams(true)
    setStreams([])
    setSelectedStream(null)
    clearDownload()

    try {
      const parameters = new URLSearchParams({ link })
      const response = await fetch(`${apiUrl}/api/VideoAndAudio/streams_info?${parameters}`)

      if (!response.ok) {
        throw new Error((await response.text()) || t.fetchStreamsError)
      }

      const availableStreams: StreamInfo[] = await response.json()
      if (availableStreams.length === 0) {
        throw new Error(t.noStreamsError)
      }

      setStreams(availableStreams)
    } catch (requestError) {
      setError(requestError instanceof Error ? requestError.message : t.requestError)
    } finally {
      setIsFetchingStreams(false)
    }
  }

  async function handlePrepareFile() {
    if (!selectedStream) return

    setError(null)
    setIsPreparingFile(true)
    clearDownload()

    try {
      const parameters = new URLSearchParams({
        link,
        id: selectedStream.id,
        format: selectedStream.videoExtension,
      })
      const endpoint = isAudioStream(selectedStream) ? 'audio_by_id' : 'video_by_id'

      if (endpoint === 'video_by_id') {
        parameters.set('resolution', getResolution(selectedStream))
      }

      const response = await fetch(`${apiUrl}/api/VideoAndAudio/${endpoint}?${parameters}`)

      if (!response.ok) {
        throw new Error((await response.text()) || t.prepareFileError)
      }

      const blob = await response.blob()
      setFileName(getFileName(response.headers.get('content-disposition'), `media.${selectedStream.videoExtension.toLowerCase()}`))
      setDownloadUrl(URL.createObjectURL(blob))
    } catch (requestError) {
      setError(requestError instanceof Error ? requestError.message : t.requestError)
    } finally {
      setIsPreparingFile(false)
    }
  }

  return (
    <main className="flex min-h-full flex-1 items-center justify-center px-5 py-12 sm:px-10">
      <section className="w-full max-w-3xl rounded-3xl border border-[var(--border)] bg-[var(--bg)] p-6 text-left shadow-[var(--shadow)] sm:p-10">
        <div className="mb-8">
          <div className="mb-6 flex justify-end gap-1" aria-label={t.language}>
            <button className={`rounded-lg px-3 py-1 text-sm font-medium transition ${locale === 'en' ? 'bg-[var(--accent-bg)] text-[var(--text-h)]' : 'text-[var(--text)] hover:bg-[var(--accent-bg)]'}`} type="button" onClick={() => { setLocale('en'); setTranslations(initialTranslations) }} aria-pressed={locale === 'en'}>English</button>
            <button className={`rounded-lg px-3 py-1 text-sm font-medium transition ${locale === 'ru' ? 'bg-[var(--accent-bg)] text-[var(--text-h)]' : 'text-[var(--text)] hover:bg-[var(--accent-bg)]'}`} type="button" onClick={() => setLocale('ru')} aria-pressed={locale === 'ru'}>Русский</button>
          </div>
          <div className="text-center">
          <p className="mb-3 text-sm font-medium uppercase tracking-[0.18em] text-[var(--accent)]">Media Downloader</p>
          <h1 className="m-0 text-4xl sm:text-5xl">{t.downloadMedia}</h1>
          <p className="mt-4 text-base">{t.description}</p>
          </div>
        </div>

        <form className="space-y-4" onSubmit={handleFetchStreams}>
          <label className="block">
            <span className="mb-2 block text-sm font-medium text-[var(--text-h)]">{t.mediaLink}</span>
            <input className="w-full rounded-xl border border-[var(--border)] bg-transparent px-4 py-3 text-[var(--text-h)] outline-none transition focus:border-[var(--accent)] focus:ring-2 focus:ring-[var(--accent-bg)]" type="url" value={link} onChange={(event) => handleLinkChange(event.target.value)} placeholder="https://www.youtube.com/watch?v=..." required />
          </label>
          <button className="w-full rounded-xl bg-[var(--accent)] px-5 py-3 font-semibold text-white transition hover:brightness-110 disabled:cursor-not-allowed disabled:opacity-60" type="submit" disabled={isFetchingStreams || isPreparingFile}>
            {isFetchingStreams ? t.fetchingStreams : t.fetchStreams}
          </button>
        </form>

        {streams.length > 0 && (
          <div className="mt-8">
            <h2 className="m-0 text-xl text-[var(--text-h)]">{t.selectFormat}</h2>
            <div className="mt-4 grid gap-3">
              {streams.map((stream) => {
                const selected = selectedStream?.id === stream.id
                const details = [
                  stream.resolution.trim(),
                  stream.videoExtension.toUpperCase(),
                  getBitrateLabel(stream.totalBitrate),
                  !isAudioStream(stream) ? stream.videoCodec : stream.audioCodec,
                ].filter((value): value is string => Boolean(value))

                return (
                  <label key={stream.id} className="cursor-pointer">
                    <input className="peer sr-only" type="radio" name="stream" checked={selected} onChange={() => { setSelectedStream(stream); setError(null); clearDownload() }} />
                    <span className="block rounded-xl border border-[var(--border)] px-4 py-4 transition peer-checked:border-[var(--accent)] peer-checked:bg-[var(--accent-bg)]">
                      <span className="flex items-center justify-between gap-4">
                        <span className="font-semibold text-[var(--text-h)]">{isAudioStream(stream) ? t.audio : t.video}</span>
                        <span className="text-sm text-[var(--text)]">ID: {stream.id}</span>
                      </span>
                      <span className="mt-2 block text-sm">{details.join(' · ')}</span>
                    </span>
                  </label>
                )
              })}
            </div>
            <button className="mt-5 w-full rounded-xl bg-[var(--accent)] px-5 py-3 font-semibold text-white transition hover:brightness-110 disabled:cursor-not-allowed disabled:opacity-60" type="button" onClick={handlePrepareFile} disabled={!selectedStream || isPreparingFile || isFetchingStreams}>
              {isPreparingFile ? t.preparingFile : t.prepareFile}
            </button>
          </div>
        )}

        {error && <p className="mt-5 rounded-xl bg-red-500/10 px-4 py-3 text-sm text-red-600 dark:text-red-400">{error}</p>}

        {downloadUrl && <a className="mt-5 block w-full rounded-xl border border-[var(--accent)] bg-[var(--accent-bg)] px-5 py-3 text-center font-semibold text-[var(--text-h)] transition hover:brightness-95" href={downloadUrl} download={fileName}>{t.downloadFile}</a>}
      </section>
    </main>
  )
}
