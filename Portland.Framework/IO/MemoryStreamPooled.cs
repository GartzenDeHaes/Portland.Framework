using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Portland.Framework.IO
{
	/// <summary>
	/// An alternative to <see cref="System.IO.MemoryStream"/> that uses a number of buffers
	/// taken from a pool as its backing store instead of a single buffer.
	/// </summary>
	public class MemoryStreamPooled : Stream
	{
		public interface IPool<T>
		{
			T Take();
			void Return(T t);
		}

		public interface IBufferPool : IPool<byte[]>
		{
			int BufferSize { get; }
		}

		/// <summary>
		/// A pool of reusable objects.
		/// </summary>
		/// <typeparam name="T">The type of objects in the pool.</typeparam>
		public class Pool<T> : IPool<T>
		{
			protected ConcurrentStack<T> _Pool = new ConcurrentStack<T>();
			protected Func<T> Allocator;

			public Pool(Func<T> allocator)
			{
				Allocator = allocator;
			}

			public T Take()
			{
				T t;
				if (!_Pool.TryPop(out t)) t = Allocator();
				return t;
			}

			public void Return(T t)
			{
				_Pool.Push(t);
			}
		}

		/// <summary>
		/// A pool of reusable byte array buffers.
		/// </summary>
		public class BufferPool : Pool<byte[]>, IBufferPool
		{
			private const int _BufferSize = 1024;

			public int BufferSize
			{
				get { return _BufferSize; }
			}

			public BufferPool()
				 : base(() => new byte[_BufferSize])
			{
			}

			public static BufferPool Instance;

			static BufferPool()
			{
				Instance = new BufferPool();
			}
		}

		private List<byte[]> Buffers = new List<byte[]>();
		private IBufferPool _pool;
		public IBufferPool BufPool
		{
			get { return _pool; }
			private set { _pool = value; }
		}

		public MemoryStreamPooled(IBufferPool pool = null)
		{
			if (pool == null)
				pool = BufferPool.Instance;
			BufPool = pool;
		}

		public MemoryStreamPooled(int capacity, IBufferPool pool = null)
		{
			if (pool == null) pool = BufferPool.Instance;
			BufPool = pool;
			SetCapacity(capacity);
		}

		public MemoryStreamPooled(byte[] buffer, IBufferPool pool = null)
		{
			Constructor(buffer, 0, buffer.Length, true, false, pool);
		}

		public MemoryStreamPooled(byte[] buffer, bool writable, IBufferPool pool = null)
		{
			Constructor(buffer, 0, buffer.Length, writable, false, pool);
		}

		public MemoryStreamPooled(byte[] buffer, int index, int count, IBufferPool pool = null)
		{
			Constructor(buffer, index, count, true, false, pool);
		}

		public MemoryStreamPooled(byte[] buffer, int index, int count, bool writable, IBufferPool pool = null)
		{
			Constructor(buffer, index, count, writable, false, pool);
		}

		public MemoryStreamPooled(byte[] buffer, int index, int count, bool writable, bool publiclyVisible, IBufferPool pool = null)
		{
			Constructor(buffer, index, count, writable, publiclyVisible, pool);
		}

		private void Constructor(byte[] buffer, int index, int count, bool writable, bool publiclyVisible, IBufferPool pool)
		{
			if (buffer == null)
				throw new ArgumentNullException("buffer");
			if (index < 0)
				throw new ArgumentOutOfRangeException("index", "index is negative");
			if (count < 0)
				throw new ArgumentOutOfRangeException("count", "count is negative");
			if (buffer.Length - index < count)
				throw new ArgumentException("index+count", "size of buffer is less than index + count");

			if (pool == null) pool = BufferPool.Instance;

			BufPool = pool;
			Write(buffer, index, count);
			Position = 0;
			canWrite = writable;
			visible = publiclyVisible;
			expandable = false;
		}

		private bool expandable = true;
		private bool canWrite = true;
		private bool visible = true;

		public virtual byte[] ToArray()
		{
			var buffer = new byte[Length];
			var bufNum = 0;
			var posInBuf = 0;
			var bytesToRead = Length;
			var bytesRead = 0;
			var bytesLeft = Length;
			var offset = 0;

			while (bytesLeft > 0)
			{
				var bytesToCopy = (posInBuf + bytesLeft) < BufPool.BufferSize ? (int)bytesLeft : BufPool.BufferSize - posInBuf;
				var buf = Buffers[bufNum];
				Buffer.BlockCopy(buf, posInBuf, buffer, offset, bytesToCopy);
				Position += bytesToCopy;
				offset += bytesToCopy;
				bytesLeft -= bytesToCopy;
				bytesRead += bytesToCopy;
				bufNum++;
				posInBuf = 0;
			}

			return buffer;
		}

		public virtual byte[] GetBuffer()
		{
			if (!visible)
				throw new UnauthorizedAccessException();

			var buffer = new byte[Capacity];
			var offset = 0;

			foreach (var buf in Buffers)
			{
				Buffer.BlockCopy(buf, 0, buffer, offset, buf.Length);
				offset += buf.Length;
			}

			return buffer;
		}

		#region implemented abstract members of Stream

		public override void Flush()
		{
		}

		private void CheckIfDisposed()
		{
			if (_disposed)
				throw new ObjectDisposedException("PoolMemoryStream");
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (buffer == null)
				throw new ArgumentNullException("buffer");
			if ((offset + count) > buffer.Length)
				throw new ArgumentException("buffer too small", "buffer");
			if (offset < 0)
				throw new ArgumentException("offset must be >= 0", "offset");
			if (count < 0)
				throw new ArgumentException("count must be >= 0", "count");

			CheckIfDisposed();

			if (Position >= Length || count == 0)
				return 0;

			var bufNum = (int)Position / BufPool.BufferSize;
			var posInBuf = (int)Position - bufNum * BufPool.BufferSize;
			var bytesToRead = Math.Min(count, (int)(Length - Position));
			var bytesRead = 0;
			var bytesLeft = bytesToRead - bytesRead;

			while (bytesLeft > 0)
			{
				var bytesToCopy = (posInBuf + bytesLeft) < BufPool.BufferSize ? bytesLeft : BufPool.BufferSize - posInBuf;
				var buf = Buffers[bufNum];
				Buffer.BlockCopy(buf, posInBuf, buffer, offset, bytesToCopy);
				Position += bytesToCopy;
				offset += bytesToCopy;
				bytesLeft -= bytesToCopy;
				bytesRead += bytesToCopy;
				bufNum++;
				posInBuf = 0;
			}

			return bytesToRead;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			CheckIfDisposed();

			if (offset > (long)int.MaxValue)
				throw new ArgumentOutOfRangeException("offset out of range. " + offset);

			switch (origin)
			{
				case SeekOrigin.Current:
					offset += Position;
					break;
				case SeekOrigin.End:
					offset += Length;
					break;
				case SeekOrigin.Begin:
					break;
				default:
					throw new ArgumentException("origin", "invalid SeekOrigin");
			}

			if (offset < 0)
				throw new IOException("Attempted to seek before start of PoolMemoryStream.");

			Position = offset;

			return Position;
		}

		public int Capacity
		{
			get
			{
				CheckIfDisposed();
				return Buffers.Count * BufPool.BufferSize;
			}

			set
			{
				CheckIfDisposed();

				if (value < 0 || value < Length)
					throw new ArgumentOutOfRangeException("value", "capacity cannot be negative or smaller than length of stream.");

				SetCapacity(value);
			}
		}

		private bool dirty = true;

		public override void SetLength(long value)
		{
			if (!canWrite)
				throw new NotSupportedException("cannot write to stream");

			SetCapacity(value);

			if (value < _length)
				dirty = true;

			_length = value;

			if (Position > _length)
				Position = _length;
		}

		private void SetCapacity(long value)
		{
			CheckIfDisposed();

			if (!expandable && value > Capacity)
				throw new NotSupportedException("cannot expand stream");

			if (value < 0 || value > int.MaxValue)
				throw new ArgumentOutOfRangeException();

			if (value == 0)
			{
				foreach (var buf in Buffers)
				{
					BufPool.Return(buf);
				}
				Buffers.Clear();
				return;
			}

			if (value == Capacity) return;

			int buffers = (int)(value / BufPool.BufferSize);
			if ((value - buffers * BufPool.BufferSize) > 0)
				buffers++;

			if (buffers < Buffers.Count)
			{
				for (int i = buffers; i < Buffers.Count; i++)
				{
					BufPool.Return(Buffers[i]);
				}

				Buffers.RemoveRange(buffers, Buffers.Count - buffers);
			}
			else
			{
				if (dirty && Buffers.Count > 0)
				{
					var dirtyBytes = Capacity - Length;
					var lastBuf = Buffers[Buffers.Count - 1];
					for (var i = BufPool.BufferSize - dirtyBytes; i < BufPool.BufferSize; i++)
					{
						lastBuf[i] = 0;
					}

					dirty = false;
				}

				if (buffers > Buffers.Count)
				{
					for (int i = Buffers.Count; i < buffers; i++)
					{
						var buf = BufPool.Take();
						Buffers.Add(buf);
					}
				}
			}
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (!canWrite)
				throw new NotSupportedException("cannot write to stream");
			if (buffer == null)
				throw new ArgumentNullException("buffer");
			if ((offset + count) > buffer.Length)
				throw new ArgumentException("buffer too small", "buffer");
			if (offset < 0)
				throw new ArgumentException("offset must be >= 0", "offset");
			if (count < 0)
				throw new ArgumentException("count must be >= 0", "count");

			CheckIfDisposed();

			if (Position > Length - count)
				SetLength(Position + count);

			var bufNum = (int)Position / BufPool.BufferSize;
			var posInBuf = (int)Position - bufNum * BufPool.BufferSize;
			var bytesWritten = 0;
			var bytesLeft = count - bytesWritten;

			while (bytesLeft > 0)
			{
				var bytesToCopy = (posInBuf + bytesLeft) < BufPool.BufferSize ? bytesLeft : BufPool.BufferSize - posInBuf;
				var buf = Buffers[bufNum];
				Buffer.BlockCopy(buffer, offset, buf, posInBuf, bytesToCopy);
				Position += bytesToCopy;
				offset += bytesToCopy;
				bytesLeft -= bytesToCopy;
				bytesWritten += bytesToCopy;
				bufNum++;
				posInBuf = 0;
			}
		}

		public override void WriteByte(byte value)
		{
			if (!canWrite)
				throw new NotSupportedException("cannot write to stream");

			CheckIfDisposed();

			if (Position >= Length)
				SetLength(Position + 1);

			var bufNum = (int)Position / BufPool.BufferSize;
			var posInBuf = (int)Position - bufNum * BufPool.BufferSize;

			var buf = Buffers[bufNum];
			buf[posInBuf] = value;

			Position++;
		}

		public override int ReadByte()
		{
			CheckIfDisposed();

			if (Position >= Length)
				return -1;

			var bufNum = (int)Position / BufPool.BufferSize;
			var posInBuf = (int)Position - bufNum * BufPool.BufferSize;

			var buf = Buffers[bufNum];
			Position++;

			return buf[posInBuf];
		}

		public bool AvailableData
		{
			get { return Position < Length; }
		}

		public override bool CanRead
		{
			get
			{
				return !_disposed;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return !_disposed;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return !_disposed && canWrite;
			}
		}

		private long _length;

		public override long Length
		{
			get
			{
				CheckIfDisposed();
				return _length;
			}
		}

		private long _position;

		public override long Position
		{
			get
			{
				CheckIfDisposed();
				return _position;
			}
			set
			{
				CheckIfDisposed();
				_position = value;
			}
		}

		#endregion

		private bool _disposed;

		protected override void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					foreach (var buffer in Buffers)
					{
						BufPool.Return(buffer);
					}

					Buffers.Clear();
				}

				_disposed = true;
			}
		}
	}
}
