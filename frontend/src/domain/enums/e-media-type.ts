export const EMediaType = {
  Audio: 0,
  Video: 1,
  Image: 2,
  Other: 3,
} as const

export type EMediaType = (typeof EMediaType)[keyof typeof EMediaType]
