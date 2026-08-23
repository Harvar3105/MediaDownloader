export const EMediaSource = {
  YouTube: 'YouTube',
  SoundCloud: 'SoundCloud',
  Facebook: 'Facebook',
  Instagram: 'Instagram',
  Twitter: 'Twitter',
  TikTok: 'TikTok',
  Reddit: 'Reddit',
  Pinterest: 'Pinterest',
} as const

export type EMediaSource = (typeof EMediaSource)[keyof typeof EMediaSource]
