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
      _logger.LogError($"Error getting streams info: {ex.Message}");
      return BadRequest(ex.Message);
    }
  }

  [HttpGet("get_mdedia_metadata")]
  public async Task<IActionResult> GetMediaMetadata(string link, string format)
  {
    try
    {
      var metadata = await _downloader.GetMediaMetadataAsync(link, format);
      return Ok(metadata);
    }
    catch (Exception ex)
    {
      _logger.LogError($"Error getting media metadata: {ex.Message}");
      return BadRequest(ex.Message);
    }
  }

  [HttpGet("video_by_id")]
  public async Task<IActionResult> GetVideoById(string link, string id, EVideoResolution resolution, EVideoExtension format)
  {
    try
    {
      var video = await _downloader.GetVideoByIdAsync(link, id, resolution, format);
      return File(video.Content, "application/octet-stream", video.Metadata.FullName);
    }
    catch (Exception ex)
    {
      return BadRequest(ex.Message);
    }
  }

  [HttpGet("audio_by_id")]
  public async Task<IActionResult> GetAudioById(string link, string id, EAudioExtension format)
  {
    try
    {
      var audio = await _downloader.GetAudioByIdAsync(link, id, format);
      return File(audio.Content, "application/octet-stream", audio.Metadata.FullName);
    }
    catch (Exception ex)
    {
      return BadRequest(ex.Message);
    }
  }
}