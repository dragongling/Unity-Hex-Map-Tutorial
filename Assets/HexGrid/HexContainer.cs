using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class HexContainer<T>
{
    private readonly T[] items;
    public readonly int sizeX, sizeZ;

    public HexContainer(int sizeX, int sizeZ){
        if (sizeX < 0 || sizeZ < 0)
            throw new ArgumentOutOfRangeException();
        this.sizeX = sizeX;
        this.sizeZ = sizeZ;
        items = new T[sizeZ * sizeX];
    }

    public T this[int x, int z] {
        get { return Get(x, z); }
        set { Set(x, z, value); }
    }

    public T this[HexCoordinates coords] {
        get { return Get(coords); }
        set { Set(coords, value); }
    }

    public T Get(HexCoordinates coords)
    {
        return Get(coords.X, coords.Z);
    }   

    public T Get(int x, int z)
    {
        CheckBounds(ref x, ref z);
        return items[x + z * sizeX];
    }

    public void Set(HexCoordinates coords, T value)
    {
        Set(coords.X, coords.Z, value);
    }

    public void Set(int x, int z, T value)
    {
        CheckBounds(ref x, ref z);
        items[x + z * sizeX] = value;
    }

    private void CheckBounds(ref int x, ref int z)
    {
        if (z < 0 || z >= sizeZ)
        {
            throw new IndexOutOfRangeException();
        }
        x += z / 2;
        if (x < 0 || x >= sizeX)
        {
            throw new IndexOutOfRangeException();
        }
    }

    public void Clear(int itemSize)
    {
        Array.Clear(items, 0, itemSize * sizeZ * sizeX);
    }
}    
