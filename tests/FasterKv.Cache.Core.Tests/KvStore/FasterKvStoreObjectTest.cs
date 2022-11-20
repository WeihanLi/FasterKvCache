﻿using FasterKv.Cache.Core.Configurations;
using FasterKv.Cache.MessagePack;
using FasterKv.Cache.SystemTextJson;

namespace FasterKv.Cache.Core.Tests.KvStore;

public class FasterKvStoreObjectTest
{
    private readonly FasterKvCache _fasterKv;

    private readonly Data _data = new()
    {
        One = "one",
        Two = 2
    };

    public FasterKvStoreObjectTest()
    {
        _fasterKv = CreateKvStore();
    }

    private static FasterKvCache CreateKvStore()
    {
        return new FasterKvCache(null!,
            new DefaultSystemClock(),
            new FasterKvCacheOptions
            {
                SerializerName = "MessagePack",
                ExpiryKeyScanInterval = TimeSpan.Zero,
                IndexCount = 16384,
                MemorySizeBit = 10,
                PageSizeBit = 10,
                ReadCacheMemorySizeBit = 10,
                ReadCachePageSizeBit = 10
            },
            new IFasterKvCacheSerializer[]
            {
                new MessagePackFasterKvCacheSerializer
                {
                    Name = "MessagePack"
                },
                new SystemTextJsonFasterKvCacheSerializer
                {
                    Name = "SystemTextJson"
                }
            },
            null);
    }

    [Fact]
    public void Set_Null_Value_Should_Get_Null_Value()
    {
        var guid = Guid.NewGuid().ToString("N");
        _fasterKv.Set<Data>(guid, null);

        var result = _fasterKv.Get<Data>(guid);
        Assert.Null(result);
    }

    [Fact]
    public void Set_Key_Should_Success()
    {
        var guid = Guid.NewGuid().ToString("N");
        _fasterKv.Set(guid, _data);

        var result = _fasterKv.Get<Data>(guid);

        Assert.Equal(_data, result);
    }

    [Fact]
    public void Set_Key_With_ExpiryTime_Should_Success()
    {
        var guid = Guid.NewGuid().ToString("N");
        _fasterKv.Set(guid, _data, TimeSpan.FromMinutes(1));

        var result = _fasterKv.Get<Data>(guid);
        
        Assert.Equal(_data, result);
    }
    

    [Fact]
    public void Get_Not_Exist_Key_Should_Return_Null()
    {
        var guid = Guid.NewGuid().ToString("N");
        var result = _fasterKv.Get<Data>(guid);
        Assert.Null(result);
    }

    [Fact]
    public void Delete_Key_Should_Success()
    {
        var guid = Guid.NewGuid().ToString("N");
        _fasterKv.Set(guid, _data);
        _fasterKv.Delete(guid);

        var result = _fasterKv.Get<Data>(guid);
        Assert.Null(result);
    }
    
    [Fact]
    public async Task SetAsync_Null_Value_Should_Get_Null_Value()
    {
        var guid = Guid.NewGuid().ToString("N");
        await _fasterKv.SetAsync<Data>(guid, null); 

        var result = await _fasterKv.GetAsync<Data>(guid);
        Assert.Null(result);
    }

    [Fact]
    public async Task SetAsync_Key_Should_Success()
    {
        var guid = Guid.NewGuid().ToString("N");
        await _fasterKv.SetAsync(guid, _data);

        var result = await _fasterKv.GetAsync<Data>(guid);

        Assert.Equal(_data, result);
    }
    
    [Fact]
    public async Task SetAsync_Key_With_ExpiryTime_Should_Success()
    {
        var guid = Guid.NewGuid().ToString("N");
        await _fasterKv.SetAsync(guid, _data, TimeSpan.FromMinutes(1));

        var result = await _fasterKv.GetAsync<Data>(guid);
        
        Assert.Equal(_data, result);
    }
    

    [Fact]
    public async Task GetAsync_Not_Exist_Key_Should_Return_Null()
    {
        var guid = Guid.NewGuid().ToString("N");
        var result = await _fasterKv.GetAsync<Data>(guid);
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_Key_Should_Success()
    {
        var guid = Guid.NewGuid().ToString("N");
        await _fasterKv.SetAsync(guid, _data);
        await _fasterKv.DeleteAsync(guid);

        var result = await _fasterKv.GetAsync<Data>(guid);
        Assert.Null(result);
    }

    [Fact]
    public void Set_Big_DataSize_Should_Success()
    {
        int nums = 10000;
        for (int i = 0; i < nums; i++)
        {
            _fasterKv.Set($"big_data_{i}", new Data
            {
                One = i.ToString(),
                Two = i
            });
        }

        for (int i = 0; i < nums; i++)
        {
            var value = _fasterKv.Get<Data>($"big_data_{i}");
            Assert.NotNull(value);
            Assert.Equal(i.ToString(), value!.One);
            Assert.Equal(i, value.Two);
        }
    }

    [Fact]
    public void Set_Big_DataSize_And_Repeat_Reading_Should_Success()
    {
        int nums = 10000;
        for (int i = 0; i < nums; i++)
        {
            _fasterKv.Set($"big_data_{i}", new Data
            {
                One = i.ToString(),
                Two = i
            });
        }
        
        var value = _fasterKv.Get<Data>($"big_data_{0}");
        Assert.NotNull(value);
        Assert.Equal(0.ToString(), value!.One);
        Assert.Equal(0, value.Two);


        value = _fasterKv.Get<Data>($"big_data_{0}");
        Assert.NotNull(value);
        Assert.Equal(0.ToString(), value!.One);
        Assert.Equal(0, value.Two);
    }
    
}