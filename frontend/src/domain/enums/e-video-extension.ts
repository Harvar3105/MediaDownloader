export const EVideoExtension = {
  Avi: 'Avi',
  Flv: 'Flv',
  Gif: 'Gif',
  Mkv: 'Mkv',
  Mov: 'Mov',
  Mp4: 'Mp4',
  Webm: 'Webm',
  Aac: 'Aac',
  Aiff: 'Aiff',
  Alac: 'Alac',
  Flac: 'Flac',
  M4a: 'M4a',
  Mka: 'Mka',
  Mp3: 'Mp3',
  Ogg: 'Ogg',
  Opus: 'Opus',
  Vorbis: 'Vorbis',
  Wav: 'Wav',
} as const

export type EVideoExtension = (typeof EVideoExtension)[keyof typeof EVideoExtension]
