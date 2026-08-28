import { useEffect, useState } from 'react'
import { EAudioExtension } from './domain/enums/e-audio-extension'
import { EVideoExtension } from './domain/enums/e-video-extension'
import { EVideoResolution } from './domain/enums/e-video-resolution'

type MediaKind = 'video' | 'audio'

const audioFormats = Object.values(EAudioExtension)
const videoFormats = Object.values(EVideoExtension)
const videoResolutions = Object.values(EVideoResolution)
const apiUrl = `http://localhost:${import.meta.env.DEV ? '5003' : '5000'}`

function getFileName(contentDisposition: string | null, fallback: string) {
  const match = contentDisposition?.match(/filename\*?=(?:UTF-8'')?["']?([^;"']+)/i)
  return match ? decodeURIComponent(match[1]) : fallback
}

export default function Home() {
  const [link, setLink] = useState('')
  const [mediaKind, setMediaKind] = useState<MediaKind>('video')
  const [videoFormat, setVideoFormat] = useState(EVideoExtension.Mp4)
  const [audioFormat, setAudioFormat] = useState(EAudioExtension.Mp3)
  const [resolution, setResolution] = useState(EVideoResolution.P720)
  const [downloadUrl, setDownloadUrl] = useState<string | null>(null)
  const [fileName, setFileName] = useState('media-download')
  const [error, setError] = useState<string | null>(null)
  const [isLoading, setIsLoading] = useState(false)

  useEffect(() => () => {
    if (downloadUrl) URL.revokeObjectURL(downloadUrl)
  }, [downloadUrl])

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setError(null)
    setIsLoading(true)

    if (downloadUrl) {
      URL.revokeObjectURL(downloadUrl)
      setDownloadUrl(null)
    }

    const parameters = new URLSearchParams({
      link,
      format: mediaKind === 'video' ? videoFormat : audioFormat,
    })

    if (mediaKind === 'video') parameters.set('resolution', String(resolution))

    try {
      const response = await fetch(`${apiUrl}/api/VideoAndAudio/${mediaKind}?${parameters}`)

      if (!response.ok) {
        throw new Error((await response.text()) || 'Could not prepare a file.')
      }

      const blob = await response.blob()
      setFileName(getFileName(response.headers.get('content-disposition'), `media.${parameters.get('format')?.toLowerCase()}`))
      setDownloadUrl(URL.createObjectURL(blob))
    } catch (requestError) {
      setError(requestError instanceof Error ? requestError.message : 'Could not send a request.')
    } finally {
      setIsLoading(false)
    }
  }

  return (
    <main className="flex min-h-full flex-1 items-center justify-center px-5 py-12 sm:px-10">
      <section className="w-full max-w-2xl rounded-3xl border border-[var(--border)] bg-[var(--bg)] p-6 text-left shadow-[var(--shadow)] sm:p-10">
        <div className="mb-8 text-center">
          <p className="mb-3 text-sm font-medium uppercase tracking-[0.18em] text-[var(--accent)]">Media Downloader</p>
          <h1 className="m-0 text-4xl sm:text-5xl">Скачать медиафайл</h1>
          <p className="mt-4 text-base">Вставьте ссылку, выберите формат — мы подготовим файл для скачивания.</p>
        </div>

        <form className="space-y-6" onSubmit={handleSubmit}>
          <label className="block">
            <span className="mb-2 block text-sm font-medium text-[var(--text-h)]">Ссылка на файл</span>
            <input className="w-full rounded-xl border border-[var(--border)] bg-transparent px-4 py-3 text-[var(--text-h)] outline-none transition focus:border-[var(--accent)] focus:ring-2 focus:ring-[var(--accent-bg)]" type="url" value={link} onChange={(event) => setLink(event.target.value)} placeholder="https://www.youtube.com/watch?v=..." required />
          </label>

          <fieldset>
            <legend className="mb-2 text-sm font-medium text-[var(--text-h)]">Тип файла</legend>
            <div className="grid grid-cols-2 gap-3">
              {(['video', 'audio'] as const).map((kind) => (
                <label key={kind} className="cursor-pointer">
                  <input className="peer sr-only" type="radio" name="media-kind" checked={mediaKind === kind} onChange={() => setMediaKind(kind)} />
                  <span className="block rounded-xl border border-[var(--border)] px-4 py-3 text-center font-medium transition peer-checked:border-[var(--accent)] peer-checked:bg-[var(--accent-bg)] peer-checked:text-[var(--text-h)]">
                    {kind === 'video' ? 'Видео' : 'Аудио'}
                  </span>
                </label>
              ))}
            </div>
          </fieldset>

          <div className="grid gap-5 sm:grid-cols-2">
            <label className="block">
              <span className="mb-2 block text-sm font-medium text-[var(--text-h)]">Формат</span>
              <select className="w-full rounded-xl border border-[var(--border)] bg-[var(--bg)] px-4 py-3 text-[var(--text-h)] outline-none focus:border-[var(--accent)]" value={mediaKind === 'video' ? videoFormat : audioFormat} onChange={(event) => mediaKind === 'video' ? setVideoFormat(event.target.value as typeof videoFormat) : setAudioFormat(event.target.value as typeof audioFormat)}>
                {(mediaKind === 'video' ? videoFormats : audioFormats).map((format) => <option key={format} value={format}>{format.toUpperCase()}</option>)}
              </select>
            </label>

            {mediaKind === 'video' && (
              <label className="block">
                <span className="mb-2 block text-sm font-medium text-[var(--text-h)]">Качество</span>
                <select className="w-full rounded-xl border border-[var(--border)] bg-[var(--bg)] px-4 py-3 text-[var(--text-h)] outline-none focus:border-[var(--accent)]" value={resolution} onChange={(event) => setResolution(Number(event.target.value) as typeof resolution)}>
                  {videoResolutions.map((value) => <option key={value} value={value}>{value}p</option>)}
                </select>
              </label>
            )}
          </div>

          {error && <p className="rounded-xl bg-red-500/10 px-4 py-3 text-sm text-red-600 dark:text-red-400">{error}</p>}

          <button className="w-full rounded-xl bg-[var(--accent)] px-5 py-3 font-semibold text-white transition hover:brightness-110 disabled:cursor-not-allowed disabled:opacity-60" type="submit" disabled={isLoading}>
            {isLoading ? 'Подготавливаем файл…' : 'Получить файл'}
          </button>
        </form>

        {downloadUrl && <a className="mt-5 block w-full rounded-xl border border-[var(--accent)] bg-[var(--accent-bg)] px-5 py-3 text-center font-semibold text-[var(--text-h)] transition hover:brightness-95" href={downloadUrl} download={fileName}>Скачать файл</a>}
      </section>
    </main>
  )
}
