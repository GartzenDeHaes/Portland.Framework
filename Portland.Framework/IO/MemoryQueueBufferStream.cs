﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Portland.Framework.IO
{
	/// <summary>
	/// This stream maintains data only until the data is read, then it is purged from the stream.
	/// </summary>
	public class MemoryQueueBufferStream : Stream
	{
		/// <summary>
		/// Represents a single write into the MemoryQueueBufferStream.  Each write is a seperate chunk
		/// </summary>
		private class Chunk
		{
			/// <summary>
			/// As we read through the chunk, the start index will increment.  When we get to the end of the chunk,
			/// we will remove the chunk
			/// </summary>
			public int ChunkReadStartIndex { get; set; }

			/// <summary>
			/// Actual Data
			/// </summary>
			public byte[] Data { get; set; }
		}

		//Maintains the streams data.  The Queue object provides an easy and efficient way to add and remove data
		//Each item in the queue represents each write to the stream.  Every call to write translates to an item in the queue
#if UNITY_2019_4_OR_NEWER
		private readonly Queue<Chunk> lstBuffers_m = new Queue<Chunk>();
#else
		private readonly ConcurrentQueue<Chunk> lstBuffers_m = new ConcurrentQueue<Chunk>();
#endif

		public MemoryQueueBufferStream()
		{
		}

		/// <summary>
		/// Reads up to count bytes from the stream, and removes the read data from the stream.
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="offset"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public override int Read(byte[] buffer, int offset, int count)
		{
			this.ValidateBufferArgs(buffer, offset, count);

			int iRemainingBytesToRead = count;

			int iTotalBytesRead = 0;

			//Read until we hit the requested count, or until we hav nothing left to read
			while (iTotalBytesRead <= count && lstBuffers_m.Count > 0)
			{
#if UNITY_2019_4_OR_NEWER
				Chunk chunk = lstBuffers_m.Peek();
#else
				//Get first chunk from the queue
				if (! lstBuffers_m.TryPeek(out Chunk chunk))
				{
					Thread.Yield();
					continue;
				}
#endif

				//Determine how much of the chunk there is left to read
				int iUnreadChunkLength = chunk.Data.Length - chunk.ChunkReadStartIndex;

				//Determine how much of the unread part of the chunk we can actually read
				int iBytesToRead = Math.Min(iUnreadChunkLength, iRemainingBytesToRead);

				if (iBytesToRead > 0)
				{
					//Read from the chunk into the buffer
					Buffer.BlockCopy(chunk.Data, chunk.ChunkReadStartIndex, buffer, offset + iTotalBytesRead, iBytesToRead);

					iTotalBytesRead += iBytesToRead;
					iRemainingBytesToRead -= iBytesToRead;

					//If the entire chunk has been read,  remove it
					if (chunk.ChunkReadStartIndex + iBytesToRead >= chunk.Data.Length)
					{
#if UNITY_2019_4_OR_NEWER
						lstBuffers_m.Dequeue();
#else
						while (! lstBuffers_m.TryDequeue(out chunk))
						{
							Thread.Yield();
						}
#endif
					}
					else
					{
						//Otherwise just update the chunk read start index, so we know where to start reading on the next call
						chunk.ChunkReadStartIndex = chunk.ChunkReadStartIndex + iBytesToRead;
					}
				}
				else
				{
					break;
				}
			}

			return iTotalBytesRead;
		}

		private void ValidateBufferArgs(byte[] buffer, int offset, int count)
		{
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "offset must be non-negative");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "count must be non-negative");
			}
			if ((buffer.Length - offset) < count)
			{
				throw new ArgumentException("requested count exceeds available size");
			}
		}

		/// <summary>
		/// Writes data to the stream
		/// </summary>
		/// <param name="buffer">Data to copy into the stream</param>
		/// <param name="offset"></param>
		/// <param name="count"></param>
		public override void Write(byte[] buffer, int offset, int count)
		{
			this.ValidateBufferArgs(buffer, offset, count);

			//We don't want to use the buffer passed in, as it could be altered by the caller
			byte[] bufSave = new byte[count];
			Buffer.BlockCopy(buffer, offset, bufSave, 0, count);

			//Add the data to the queue
			this.lstBuffers_m.Enqueue(new Chunk() { ChunkReadStartIndex = 0, Data = bufSave });
		}

		public override bool CanSeek
		{
			get { return false; }
		}

		/// <summary>
		/// Always returns 0
		/// </summary>
		public override long Position
		{
			get
			{
				//We're always at the start of the stream, because as the stream purges what we've read
				return 0;
			}
			set
			{
				throw new NotSupportedException(this.GetType().Name + " is not seekable");
			}
		}

		public override bool CanWrite
		{
			get { return true; }
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException(this.GetType().Name + " is not seekable");
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException(this.GetType().Name + " length can not be changed");
		}

		public override bool CanRead
		{
			get { return true; }
		}

		public bool AvailableToRead
		{
			get { return lstBuffers_m.Count > 0; }
		}

		public override long Length
		{
			get
			{

				if (this.lstBuffers_m == null)
				{
					return 0;
				}

				if (this.lstBuffers_m.Count == 0)
				{
					return 0;
				}

				return this.lstBuffers_m.Sum(b => b.Data.Length - b.ChunkReadStartIndex);
			}
		}

		public override void Flush()
		{
			lstBuffers_m.Clear();
		}
	}
}
