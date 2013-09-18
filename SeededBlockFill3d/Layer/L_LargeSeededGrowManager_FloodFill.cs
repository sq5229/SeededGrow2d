using System;
using System.Collections.Generic;
using System.Text;
using SeededBlockFill3d.Layer.FF;

namespace SeededBlockFill3d.Layer
{
    public class L_LargeSeededGrowManager_FloodFill
    {
        #region Static
        struct LayerDataAndInput
        {
            public Layer layer;
            public FloodFillInput input;
            public LayerDataAndInput(Layer layer, FloodFillInput input)
            {
                this.layer = layer;
                this.input = input;
            }
        }
        #endregion
        L_LargeImage image;
        Container_Stack<LayerDataAndInput> queue;
        byte[] buffer;
        L_FloodFillBase seedGrowExecutor;
        DataFiller dataProvider;
        public List<Int16Triple> resultSet = new List<Int16Triple>();
        public int resultCount = 0;
        public L_LargeSeededGrowManager_FloodFill()
        {

        }
        public void SetScale(int width, int height, int depth, int subDepth)
        {
            this.image = new L_LargeImage(width, height, depth);
            SetLayerSize(subDepth);
        }
        private void SetLayerSize(int subdepth)
        {
            image.CreateLayers(subdepth);
            buffer = new byte[image.GetWidth() * image.GetHeight() * subdepth];
        }
        public void SetExecutor(L_FloodFillBase grower)
        {
            this.seedGrowExecutor = grower;
        }
        public void SetDataProvider(DataFiller df)
        {
            dataProvider = df;
        }
        public void ExecuteSeededGrow(Int16Triple firstseed)
        {
            if (buffer == null || seedGrowExecutor == null || dataProvider == null)
                throw new Exception();
            queue = new Container_Stack<LayerDataAndInput>();
            Layer firstLayer = GetFirstLayer(firstseed);
            FloodFillInput firstInput = new FloodFillInput
                (buffer, firstLayer.AllWidth, firstLayer.AllHeight, firstLayer.actualDepth, firstLayer.GetAndInitFlag(),
                new List<Int16Triple>() { ConvertGlobalCoodToLayerCoold(firstLayer, firstseed) },
                firstLayer.HasUpperLayer(), firstLayer.HasLowerLayer(), true);
            queue.Push(new LayerDataAndInput(firstLayer, firstInput));
            while (!queue.Empty())
            {
                LayerDataAndInput sgi = queue.Pop();
                Layer layer = sgi.layer;
                layer.visitcount++;
                FloodFillInput input = sgi.input;
                FillLayerData(layer);
                FloodFillResult ret1 = seedGrowExecutor.ExecuteSeededGrow(input);
                input.overstepList.Clear();
                //ConvertLayerCoordsToGlobalCoords(layer,ret1.resultPointSet);
                //resultSet.AddRange(ret1.resultPointSet);
                resultCount += ret1.resultCount;
                if (ret1.GetNeedsSeekLower())
                {
                    Layer lowerlayer = image.GetLayer(layer.indexZ - 1);
                    if (lowerlayer == null)
                    {
                        ret1.boundaryRequestPoints[0].Clear();
                        continue;
                    }
                    ConvertOtherLayerCoordsToThisLayerCoords(layer, lowerlayer, ret1.boundaryRequestPoints[0]);
                    FloodFillInput newinput = new FloodFillInput
                        (buffer, lowerlayer.AllWidth, lowerlayer.AllHeight, lowerlayer.actualDepth, lowerlayer.GetAndInitFlag(),
                        ret1.boundaryRequestPoints[0], lowerlayer.HasUpperLayer(), lowerlayer.HasLowerLayer(), false);
                    queue.Push(new LayerDataAndInput(lowerlayer, newinput));
                }
                if (ret1.GetNeedsSeekUpper())
                {
                    Layer upperlayer = image.GetLayer(layer.indexZ + 1);
                    if (upperlayer == null)
                    {
                        ret1.boundaryRequestPoints[1].Clear();
                        continue;
                    }
                    ConvertOtherLayerCoordsToThisLayerCoords(layer, upperlayer, ret1.boundaryRequestPoints[1]);
                    FloodFillInput newinput = new FloodFillInput
                        (buffer, upperlayer.AllWidth, upperlayer.AllHeight, upperlayer.actualDepth, upperlayer.GetAndInitFlag(),
                        ret1.boundaryRequestPoints[1], upperlayer.HasUpperLayer(), upperlayer.HasLowerLayer(), false);
                    queue.Push(new LayerDataAndInput(upperlayer, newinput));
                }
            }

        }
        private Layer GetFirstLayer(Int16Triple seed)
        {
            if (seed.X < 0 || seed.X > image.GetWidth() || seed.Y < 0 || seed.Y > image.GetHeight() || seed.Z < 0 || seed.Z > image.GetDepth())
                throw new Exception();
            int index = image.GetLayerIndex(seed);
            return image.GetLayer(index);
        }
        private void FillLayerData(Layer b)
        {
            dataProvider.LoadLayerData(buffer, b.stz, b.edz);
        }

        private Int16Triple ConvertGlobalCoodToLayerCoold(Layer thislayer, Int16Triple globalCoord)
        {
            return new Int16Triple(globalCoord.X, globalCoord.Y, globalCoord.Z - thislayer.stz);
        }
        private void ConvertLayerCoordsToGlobalCoords(Layer thislayer, List<Int16Triple> adjSeedList)
        {
            for (int i = 0; i < adjSeedList.Count; i++)
            {
                Int16Triple old = adjSeedList[i];
                old.Z += thislayer.stz;
                adjSeedList[i] = old;
            }
        }
        private void ConvertOtherLayerCoordsToThisLayerCoords(Layer thislayer, Layer other, List<Int16Triple> adjSeedList)
        {
            for (int i = 0; i < adjSeedList.Count; i++)
            {
                Int16Triple old = adjSeedList[i];
                old.Z += (thislayer.stz - other.stz);
                adjSeedList[i] = old;
            }
        }
    }
}
