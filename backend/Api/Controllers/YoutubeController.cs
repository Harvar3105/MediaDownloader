using MediaDownloader.Domain.Enums;
using MediaDownloader.Youtube;
using Microsoft.AspNetCore.Mvc;

namespace MediaDownloader.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class YoutubeController : ControllerBase
{
  private readonly YoutubeDownloader _downloader;

  public YoutubeController(YoutubeDownloader downloader)
  {
    _downloader = downloader;
  }

  [HttpGet("video")]
  public async Task<IActionResult> GetVideo(string link, EVideoResolution resolution = EVideoResolution.P720)
  {
    try
    {
      var video = await _downloader.GetVideo(link, resolution);
      return File(video.Content, "application/octet-stream", $"{video.Metadata.FullName}");
    }
    catch (Exception ex)
    {
      return BadRequest(ex.Message);
    }
  }

  [HttpGet("audio")]
  public async Task<IActionResult> GetAudio(string link)
  {
    try
    {
      var audio = await _downloader.GetAudio(link);
      return File(audio.Content, "application/octet-stream", $"{audio.Metadata.FullName}");
    }
    catch (Exception ex)
    {
      return BadRequest(ex.Message);
    }
  }
}