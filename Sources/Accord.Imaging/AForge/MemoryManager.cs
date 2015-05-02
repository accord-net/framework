// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2005-2009
// andrew.kirillov@aforgenet.com
//

namespace AForge.Imaging
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Internal memory manager used by image processing routines.
    /// </summary>
    /// 
    /// <remarks><para>The memory manager supports memory allocation/deallocation
    /// caching. Caching means that memory blocks may be not freed on request, but
    /// kept for later reuse.</para></remarks>
    /// 
    public static class MemoryManager
    {
        // maximum memory blocks to cache
        private static int maximumCacheSize = 3;
        // current cache size
        private static int currentCacheSize = 0;
        // busy blocks in cache
        private static int busyBlocks = 0;
        // amount of cached memory
        private static int cachedMemory = 0;

        // maximum block size to cache
        private static int maxSizeToCache = 20 * 1024 * 1024;
        // minimum block size to cache
        private static int minSizeToCache = 10 * 1024;

        // cache structure
        private class CacheBlock
        {
            public IntPtr   MemoryBlock;
            public int      Size;
            public bool     Free;

            public CacheBlock( IntPtr memoryBlock, int size )
            {
                this.MemoryBlock = memoryBlock;
                this.Size = size;
                this.Free = false;
            }
        }

        private static List<CacheBlock> memoryBlocks = new List<CacheBlock>( );

        /// <summary>
        /// Maximum amount of memory blocks to keep in cache.
        /// </summary>
        /// 
        /// <remarks><para>The value specifies the amount of memory blocks, which could be
        /// cached by the memory manager.</para>
        /// 
        /// <para>Default value is set to 3. Maximum value is 10.</para>
        /// </remarks>
        /// 
        public static int MaximumCacheSize
        {
            get
            {
                lock ( memoryBlocks )
                {
                    return maximumCacheSize;
                }
            }
            set
            {
                lock ( memoryBlocks )
                {
                    maximumCacheSize = Math.Max( 0, Math.Min( 10, value ) );
                }
            }
        }

        /// <summary>
        /// Current amount of memory blocks in cache.
        /// </summary>
        /// 
        public static int CurrentCacheSize
        {
            get
            {
                lock ( memoryBlocks )
                {
                    return currentCacheSize;
                }
            }
        }

        /// <summary>
        /// Amount of busy memory blocks in cache (which were not freed yet by user).
        /// </summary>
        /// 
        public static int BusyMemoryBlocks
        {
            get
            {
                lock ( memoryBlocks )
                {
                    return busyBlocks;
                }
            }
        }

        /// <summary>
        /// Amount of free memory blocks in cache (which are not busy by users).
        /// </summary>
        /// 
        public static int FreeMemoryBlocks
        {
            get
            {
                lock ( memoryBlocks )
                {
                    return currentCacheSize - busyBlocks;
                }
            }
        }

        /// <summary>
        /// Amount of cached memory in bytes.
        /// </summary>
        /// 
        public static int CachedMemory
        {
            get
            {
                lock ( memoryBlocks )
                {
                    return cachedMemory;
                }
            }
        }

        /// <summary>
        /// Maximum memory block's size in bytes, which could be cached.
        /// </summary>
        /// 
        /// <remarks>Memory blocks, which size is greater than this value, are not cached.</remarks>
        /// 
        public static int MaxSizeToCache
        {
            get { return maxSizeToCache; }
            set { maxSizeToCache = value; }
        }

        /// <summary>
        /// Minimum memory block's size in bytes, which could be cached.
        /// </summary>
        /// 
        /// <remarks>Memory blocks, which size is less than this value, are not cached.</remarks>
        /// 
        public static int MinSizeToCache
        {
            get { return minSizeToCache; }
            set { minSizeToCache = value; }
        }

        /// <summary>
        /// Allocate unmanaged memory.
        /// </summary>
        /// 
        /// <param name="size">Memory size to allocate.</param>
        /// 
        /// <returns>Return's pointer to the allocated memory buffer.</returns>
        /// 
        /// <remarks>The method allocates requested amount of memory and returns pointer to it. It may avoid allocation
        /// in the case some caching scheme is uses and there is already enough allocated memory available.</remarks>
        /// 
        /// <exception cref="OutOfMemoryException">There is insufficient memory to satisfy the request.</exception>
        /// 
        public static IntPtr Alloc( int size )
        {
            lock ( memoryBlocks )
            {
                // allocate memory block without caching if cache is not available
                if ( ( busyBlocks >= maximumCacheSize ) || ( size > maxSizeToCache ) || ( size < minSizeToCache ) )
                    return Marshal.AllocHGlobal( size );

                // if all cached blocks are busy, create new cache block
                if ( currentCacheSize == busyBlocks )
                {
                    IntPtr memoryBlock = Marshal.AllocHGlobal( size );
                    memoryBlocks.Add( new CacheBlock( memoryBlock, size ) );

                    busyBlocks++;
                    currentCacheSize++;
                    cachedMemory += size;

                    return memoryBlock;
                }

                // find free memory block with enough memory
                for ( int i = 0; i < currentCacheSize; i++ )
                {
                    CacheBlock block = memoryBlocks[i];

                    if ( ( block.Free == true ) && ( block.Size >= size ) )
                    {
                        block.Free = false;
                        busyBlocks++;
                        return block.MemoryBlock;
                    }
                }

                // finaly find first free memory block and resize it
                for ( int i = 0; i < currentCacheSize; i++ )
                {
                    CacheBlock block = memoryBlocks[i];

                    if ( block.Free == true )
                    {
                        // remove this block cache
                        Marshal.FreeHGlobal( block.MemoryBlock );
                        memoryBlocks.RemoveAt( i );
                        currentCacheSize--;
                        cachedMemory -= block.Size;

                        // add new one
                        IntPtr memoryBlock = Marshal.AllocHGlobal( size );
                        memoryBlocks.Add( new CacheBlock( memoryBlock, size ) );

                        busyBlocks++;
                        currentCacheSize++;
                        cachedMemory += size;

                        return memoryBlock;
                    }
                }

                return IntPtr.Zero;
            }
        }

        /// <summary>
        /// Free unmanaged memory.
        /// </summary>
        /// 
        /// <param name="pointer">Pointer to memory buffer to free.</param>
        /// 
        /// <remarks>This method may skip actual deallocation of memory and keep it for future <see cref="Alloc"/> requests,
        /// if some caching scheme is used.</remarks>
        /// 
        public static void Free( IntPtr pointer )
        {
            lock ( memoryBlocks )
            {
                // find the memory block in cache
                for ( int i = 0; i < currentCacheSize; i++ )
                {
                    if ( memoryBlocks[i].MemoryBlock == pointer )
                    {
                        // mark the block as free
                        memoryBlocks[i].Free = true;
                        busyBlocks--;
                        return;
                    }
                }

                // the block was not cached, so lets just free it
                Marshal.FreeHGlobal( pointer );
            }
        }

        /// <summary>
        /// Force freeing unused memory.
        /// </summary>
        /// 
        /// <remarks>Frees and removes from cache memory blocks, which are not used by users.</remarks>
        /// 
        /// <returns>Returns number of freed memory blocks.</returns>
        /// 
        public static int FreeUnusedMemory( )
        {
            lock ( memoryBlocks )
            {
                int freedBlocks = 0;

                // free all unused memory
                for ( int i = currentCacheSize - 1; i >= 0; i-- )
                {
                    if ( memoryBlocks[i].Free )
                    {
                        Marshal.FreeHGlobal( memoryBlocks[i].MemoryBlock );
                        cachedMemory -= memoryBlocks[i].Size;
                        memoryBlocks.RemoveAt( i );
                        freedBlocks++;
                    }
                }
                currentCacheSize -= freedBlocks;

                return freedBlocks;
            }
        }
    }
}
