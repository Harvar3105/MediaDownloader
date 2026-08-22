using MediaDownloader.Application;
using MediaDownloader.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace MediaDownloader.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class YoutubeController : ControllerBase
{
  private VideoAndAudioDownloader _downloader;

  public YoutubeController(VideoAndAudioDownloader downloader)
  {
    _downloader = downloader;
  }

  [HttpGet("video")]
  public async Task<IActionResult> GetVideo(string link, EVideoResolution resolution = EVideoResolution.P720, EVideoExtension format = EVideoExtension.Mp4)
  {
    try
    {
      var video = await _downloader.GetVideoAsync(link, resolution, format);
      return File(video.Content, "application/octet-stream", video.Metadata.FullName);
    }
    catch (Exception ex)
    {
      return BadRequest(ex.Message);
    }
  }

  [HttpGet("audio")]
  public async Task<IActionResult> GetAudio(string link, EAudioExtension format = EAudioExtension.Mp3)
  {
    try
    {
      var audio = await _downloader.GetAudioAsync(link, format);
      return File(audio.Content, "application/octet-stream", audio.Metadata.FullName);
    }
    catch (Exception ex)
    {
      return BadRequest(ex.Message);
    }
  }
}