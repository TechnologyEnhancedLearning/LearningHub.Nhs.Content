﻿using System.IO;
using System.Threading.Tasks;

namespace LearningHub.Nhs.Content.Interfaces
{
    using System.IO;
    using System.Threading.Tasks;
    using Azure.Storage.Files.Shares.Models;
  
    /// <summary>
    /// Defines the <see cref="IFileService" />.
    /// </summary>
    public interface IFileService
    {
      /// <summary>
      /// The DeleteChunkDirectory.
      /// </summary>
      /// <param name="directoryRef">Directory ref.</param>
      /// <param name="chunks">Chunks.</param>
      /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
      Task DeleteChunkDirectory(string directoryRef, int chunks);

      /// <summary>
      /// The DownloadFileAsync.
      /// </summary>
      /// <param name="filePath">File path.</param>
      /// <param name="fileName">File name.</param>
      /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
      Task<ShareFileDownloadInfo> DownloadFileAsync(string filePath, string fileName);

      /// <summary>
      /// The ProcessFile.
      /// </summary>
      /// <param name="fileBytes">The fileBytes<see cref="Stream"/>.</param>
      /// <param name="fileName">The fileName<see cref="string"/>.</param>
      /// <param name="directoryRef">The directoryRef<see cref="string"/>.</param>
      /// <param name="originalFileName">The originalFileName<see cref="string"/>.</param>
      /// <returns>The <see cref="string"/>.</returns>
      Task<string> ProcessFile(Stream fileBytes, string fileName, string directoryRef = "");
    }
}
