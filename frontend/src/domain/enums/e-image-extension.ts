export const EImageExtension = {
  Png: 'Png',
  Jpg: 'Jpg',
  Jpeg: 'Jpeg',
  WebP: 'WebP',
  Gif: 'Gif',
  Svg: 'Svg',
} as const

export type EImageExtension = (typeof EImageExtension)[keyof typeof EImageExtension]
