using System;
using System.Collections.Generic;
using System.Text;

namespace SeededBlockFill3d.Layer
{
    public class LargeSeededGrowManager
    {
        #region Static
        class LayerDataAndInput
        {
            public Layer layer;
            public SeededGrowInput input;
            public LayerDataAndInput(Layer layer,SeededGrowInput input)
            {
                this.layer = layer;
                this.input = input;
            }
        }
        public static Int16Triple[] Adj6IndexToAdjDelta = new Int16Triple[6] 
        {
            new Int16Triple(0, 1, 0),
            new Int16Triple(0, -1, 0),
            new Int16Triple(1, 0, 0),
            new Int16Triple(-1, 0, 0),
            new Int16Triple(0, 0, 1),
            new Int16Triple(0, 0, -1)
        };
        #endregion
        LargeImage image;
        Container_Stack<LayerDataAndInput> queue;
        byte[] buffer;
        SeededGrowBase seedGrowExecutor;
        DataFiller dataProvider;
        public List<Int16Triple> resultSet=new List<Int16Triple>();
        public LargeSeededGrowManager()
        {
           
        }
        public void SetScale(int width, int height, int depth,int subDepth)
        {
            this.image = new LargeImage(width, height, depth);
            SetLayerSize(subDepth);
        }
        private void SetLayerSize(int subdepth)
        {
            image.CreateLayers(subdepth);
            buffer = new byte[image.GetWidth() * image.GetHeight() * subdepth];
        }
        public void SetExecutor(SeededGrowBase grower)
        {
            this.seedGrowExecutor = grower;
        }
        public void SetDataProvider(DataFiller df)
        {
            dataProvider = df;
        }
        public void ExecuteSeededGrow(Int16Triple firstseed)
        {
            if (buffer == null||seedGrowExecutor==null||dataProvider==null)
                throw new Exception();
            queue = new Container_Stack<LayerDataAndInput>();
            Layer firstLayer = GetFirstLayer(firstseed);
            SeededGrowInput firstInput = new SeededGrowInput
                (buffer, firstLayer.AllWidth, firstLayer.AllHeight, firstLayer.actualDepth, firstLayer.GetAndInitFlag(), 
                new List<Int16Triple>() {firstLayer.ConvertGlobalCoodToLayerCoold(firstseed)},
                firstLayer.HasUpperLayer(),firstLayer.HasLowerLayer(),true);
            queue.Push(new LayerDataAndInput(firstLayer,firstInput));
            while (!queue.Empty())
            {
                LayerDataAndInput sgi = queue.Pop();
                Layer layer = sgi.layer;
                layer.visitcount++;
                SeededGrowInput input = sgi.input;
                FillLayerData(layer);
                SeededGrowResult ret1 = seedGrowExecutor.ExecuteSeededGrow(input);
                layer.ConvertLayerCoordsToGlobalCoords(ret1.resultPointSet);
                resultSet.AddRange(ret1.resultPointSet);
                if (ret1.GetNeedsSeekLower())
                {
                    Layer lowerlayer = image.GetLayer(layer.indexZ - 1);
                    if (lowerlayer == null)
                        continue;
                    layer.ConvertLayerCoordsToGlobalCoords(ret1.boundaryRequestPoints[0]);
                    lowerlayer.ConvertGlobalCoordsToLayerCoords(ret1.boundaryRequestPoints[0]);
                    SeededGrowInput newinput = new SeededGrowInput
                       (buffer, lowerlayer.AllWidth, lowerlayer.AllHeight, lowerlayer.actualDepth, lowerlayer.GetAndInitFlag(),
                       ret1.boundaryRequestPoints[0], lowerlayer.HasUpperLayer(), lowerlayer.HasLowerLayer(),false);
                    queue.Push(new LayerDataAndInput(lowerlayer, newinput));
                }
                if (ret1.GetNeedsSeekUpper())
                {
                    Layer upperlayer = image.GetLayer(layer.indexZ + 1);
                    if (upperlayer == null)
                        continue;
                    layer.ConvertLayerCoordsToGlobalCoords(ret1.boundaryRequestPoints[1]);
                    upperlayer.ConvertGlobalCoordsToLayerCoords(ret1.boundaryRequestPoints[1]);
                    SeededGrowInput newinput = new SeededGrowInput
                       (buffer, upperlayer.AllWidth, upperlayer.AllHeight, upperlayer.actualDepth, upperlayer.GetAndInitFlag(),
                       ret1.boundaryRequestPoints[1], upperlayer.HasUpperLayer(), upperlayer.HasLowerLayer(),false);
                    queue.Push(new LayerDataAndInput(upperlayer, newinput));
                }
            }

        }
        private Layer GetFirstLayer(Int16Triple seed)
        {
            if(seed.X<0||seed.X>image.GetWidth()||seed.Y<0||seed.Y>image.GetHeight()||seed.Z<0||seed.Z>image.GetDepth())
                throw new Exception();
            int index=image.GetLayerIndex(seed);
            return image.GetLayer(index);
        }
        private void FillLayerData(Layer b)
        {
            dataProvider.LoadLayerData(buffer, b.stz, b.edz);
        }
    }
}
