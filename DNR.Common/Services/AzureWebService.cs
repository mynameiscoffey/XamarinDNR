﻿

using System;
using System.Net.Http;
using System.Threading.Tasks;
using DNR.Portable.Models;
using Microsoft.WindowsAzure.MobileServices;

namespace DNR.Portable.Services
{
  /// <summary>
  /// Azure web service class to add/update time stamples
  /// </summary>
  public class AzureWebService
  {

    public AzureWebService()
    {
      // Update MobileServiceClient with your tables URL and Cliet Key.
      podcastClient = new MobileServiceClient(
        "https://<YOUR URL HERE>.azure-mobile.net/",
        "<YOUR CLIENT KEY HERE>");

      podcastTable = podcastClient.GetTable<PodcastEpisode>();
    }

    private MobileServiceClient podcastClient;
    private MobileServiceCollection<PodcastEpisode, PodcastEpisode> podcasts;
    private IMobileServiceTable<PodcastEpisode> podcastTable;
    static readonly AzureWebService instance = new AzureWebService();

    public MobileServiceClient Client
    {
      get { return podcastClient; }
    }

    public async Task<PodcastEpisode> GetTimeAsync(int shownum)
    {
      try
      {
        //retrieve postcast based off show number
        podcasts = await podcastTable.Where(p=>p.ShowNumber == shownum).ToCollectionAsync();
        return podcasts.Count > 0 ? podcasts[0] : null;
      }
      catch (Exception ex)
      {
      }

      return new PodcastEpisode { ShowNumber = shownum, CurrentTime = 0 };
    }
    
    public async Task<PodcastEpisode> SaveTimeAsync(PodcastEpisode ep)
    {
      try
      {
        //Check to see if the podcast already exists, if Id is set.
        //then update it, else insert.
        if (!string.IsNullOrWhiteSpace(ep.Id))
        {
          await podcastTable.UpdateAsync(ep);
        }
        else
        {
          await podcastTable.InsertAsync(ep);
        }

        //return the episode back with the updated Id
        return ep;
      }
      catch (Exception ex)
      {
      }
      return null;
    }

    /// <summary>
    /// Gets the instance of the Azure Web Service
    /// </summary>
    public static AzureWebService Instance
    {
      get
      {
        return instance;
      }
    }

  }
}
