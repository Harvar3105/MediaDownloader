export const EAudioExtension = {
  Aac: 'Aac',
  Alac: 'Alac',
  Flac: 'Flac',
  M4a: 'M4a',
  Mp3: 'Mp3',
  Opus: 'Opus',
  Vorbis: 'Vorbis',
  Wav: 'Wav',
  Webm: 'Webm',
} as const

export type EAudioExtension = (typeof EAudioExtension)[keyof typeof EAudioExtension]
