using MediaDownloader.Application;
using MediaDownloader.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace MediaDownloader.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VideoAndAudioController : ControllerBase
{
  private VideoAndAudioDownloader _downloader;
  private readonly ILogger<VideoAndAudioController> _logger;

  public VideoAndAudioController(VideoAndAudioDownloader downloader, ILogger<VideoAndAudioController> logger)
  {
    _downloader = downloader;
    _logger = logger;
  }

  [HttpGet("streams_info")]
  public async Task<IActionResult> GetStreamsInfo(string link)
  {
    try
    {
      var streamsInfo = await _downloader.GetStreamsInfoAsync(link);
      return Ok(streamsInfo);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, $"Error getting streams info: {ex.Message}");
      return BadRequest(ex.Message);
    }
  }

  [HttpGet("get_mdedia_metadata")]
  public async Task<IActionResult> GetMediaMetadata(string link, EExtension format)
  {
    try
    {
      var metadata = await _downloader.GetMediaMetadataAsync(link, format);
      return Ok(metadata);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, $"Error getting media metadata: {ex.Message}");
      return BadRequest(ex.Message);
    }
  }

  [HttpGet("video_by_id")]
  public async Task<IActionResult> GetVideoById(string link, string id, EResolution resolution = EResolution.P144, EExtension format = EExtension.Mp4)
  {
    try
    {
      var video = await _downloader.GetVideoByIdAsync(link, id, resolution, format);

      HttpContext.Response.OnCompleted(() =>
        {
          video.Delete();
          return Task.CompletedTask;
        });

      var stream = video.OpenRead();

      return File(stream, $"video/{format}", video._metadata.FullName);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failedto get the video by id: " + ex.Message);
      return BadRequest(ex.Message);
    }
  }

  [HttpGet("audio_by_id")]
  public async Task<IActionResult> GetAudioById(string link, string id, EExtension format = EExtension.Mp3)
  {
    try
    {
      var audio = await _downloader.GetAudioByIdAsync(link, id, format);

      HttpContext.Response.OnCompleted(() =>
        {
          audio.Delete();
          return Task.CompletedTask;
        });
    
      var stream = audio.OpenRead();

      return File(stream, $"audio/{format}", audio._metadata.FullName);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failedto get the audio by id: " + ex.Message);
      return BadRequest(ex.Message);
    }
  }
}
