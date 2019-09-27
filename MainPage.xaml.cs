using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using System;
using Windows.Foundation;
using Windows.Web.Http;
using Newtonsoft.Json;
using JsonTestApplication.Communications;
using JsonTestApplication.DataTypes;
using JsonTestApplication.RestApi;
using JsonTestApplication.Account;
using Windows.UI.Xaml.Shapes;
using Windows.UI;
using System.Threading.Tasks;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace JsonTestApplication
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
		private List<Candle> candles { get; set; }

		public MainPage()
        {
            this.InitializeComponent();			
		}

		private void FormLoaded(object sender, RoutedEventArgs e)
		{
			
			// Setup account data
			Account.Account account = new Account.Account();
			account.accessToken = "secret-token-from-your-oanda-account";

			// Get candle data
			HttpGet httpGet = new HttpGet();
			httpGet.accessToken = account.accessToken;
			var data = httpGet.Get("https://api-fxpractice.oanda.com/v1/candles?instrument=EUR_USD&count=30&granularity=D");
			
			// Deserialize JSON into List<Candle> object
			CandlesResponse candlesResponse = JsonConvert.DeserializeObject<CandlesResponse>(data);
			//List<Candle> candles = new List<Candle>();
			candles = new List<Candle>();
			candles.AddRange(candlesResponse.candles);
			


			canGraph.Width = this.ActualWidth;
			canGraph.Height = this.ActualHeight;

			// Draw the graph
			//DrawGraphOrig();
			DrawGraph();
		}

		private void DrawGraph()
		{
			// Get candle statics
			int count = candles.Count();
			//int count = 5;
			//double viewMax = 1.1568f;
			//double viewMin = 1.1532f;
			//double viewStep = 0.0001f;
			//double viewMax = 8.5f;
			//double viewMin = 3.5f;
			//double viewStep = 0.1f;
			//int ySteps = (int)(viewMax - viewMin);

			// Canvas
			double width = canGraph.Width;
			double height = canGraph.Height;
			double xOffset = 55.0f;
			double xMargin = 5.0f;// + xOffset/2;
			double yMargin = 45.0f; // + yOffset;


			// Y-scale
			double candleMax = 0.0f;
			double candleMin = 1000000000.0f;
			double viewHeight = canGraph.Height - 2 * yMargin;
			// Find max min of candles
			foreach (var item in candles)
			{
				if (candleMax < item.openAsk)
					candleMax = item.openAsk;
				if (candleMin > item.openAsk)
					candleMin = item.openAsk;
			}

			double viewMax = candleMax;
			double viewMin = candleMin;
			double viewStep = (candleMax-candleMin)/20;
			int ySteps = (int)(viewMax - viewMin);

			// Brushes
			SolidColorBrush brushAxis = new SolidColorBrush(Colors.Black);


			// Make the X axis
			Line xAxis = new Line();
			xAxis.Stroke = brushAxis;
			xAxis.StrokeThickness = 1;
			xAxis.X1 = xMargin;
			xAxis.Y1 = height - yMargin;
			xAxis.X2 = width - xMargin - xOffset;
			xAxis.Y2 = height - yMargin;
			canGraph.Children.Add(xAxis);

			// Make the X axis bar and text
			double xAxisBarStep = (width - xMargin * 2 - xOffset) / (count - 1);
			Line xAxisBar;
			TextBlock xAxisBarText;
			for (int x = 0; x <= (count - 1); x++)
			{
				// X axis bars
				xAxisBar = new Line();
				xAxisBar.Stroke = brushAxis;
				xAxisBar.StrokeThickness = 1;
				xAxisBar.X1 = xMargin + x * xAxisBarStep;
				xAxisBar.Y1 = height - yMargin;
				xAxisBar.X2 = xMargin + x * xAxisBarStep;
				xAxisBar.Y2 = height - yMargin + 5.0f;
				canGraph.Children.Add(xAxisBar);

				if ((x+1) % 5 == 0)
				{
					// X axis text
					xAxisBarText = new TextBlock();
					xAxisBarText.Text = (x + 1).ToString();
					xAxisBarText.Foreground = brushAxis;
					Canvas.SetLeft(xAxisBarText, xMargin + x * xAxisBarStep - 4.0f);
					Canvas.SetTop(xAxisBarText, height - yMargin + 4.0f);
					canGraph.Children.Add(xAxisBarText);
				}
			}


			// Make the Y axis
			Line yAxis = new Line();
			yAxis.Stroke = brushAxis;
			yAxis.StrokeThickness = 1;
			yAxis.X1 = width - xMargin - xOffset;
			yAxis.Y1 = yMargin - 1;
			yAxis.X2 = width - xMargin - xOffset;
			yAxis.Y2 = height - yMargin;
			canGraph.Children.Add(yAxis);

			// Make the Y axis bar and text
			double yAxisBarStep = (height - yMargin * 2) / (viewMax - viewMin); //(width - xMargin * 2) / (count - 1);
			Line yAxisBar;
			TextBlock yAxisBarText;
			for (double y = viewMin; y < viewMax + viewStep/2; y += viewStep)
			{
				// Y axis bars
				yAxisBar = new Line();
				yAxisBar.Stroke = brushAxis;
				yAxisBar.StrokeThickness = 1;
				yAxisBar.X1 = width - xMargin - xOffset;
				yAxisBar.Y1 = height - yMargin - (y - viewMin) * yAxisBarStep;
				yAxisBar.X2 = width - xMargin - xOffset + 5.0f;
				yAxisBar.Y2 = height - yMargin - (y - viewMin) * yAxisBarStep;
				canGraph.Children.Add(yAxisBar);

				// Y axis text
				yAxisBarText = new TextBlock();
				yAxisBarText.Text = String.Format("{0," + ((int)y).ToString().Length + ":N4}", y); //(y).ToString();
				yAxisBarText.Foreground = brushAxis;
				Canvas.SetLeft(yAxisBarText, width - xMargin - xOffset + 5.0f + 4.0f);
				Canvas.SetTop(yAxisBarText, height - yMargin - (y - viewMin) * yAxisBarStep - 5.0f - 6.0f);
				canGraph.Children.Add(yAxisBarText);
			}


			/*
			Line candleLine;
			for (int x = 0; x <= (count - 1); x++)
			{
				// Candle line
				candleLine = new Line();
				candleLine.Stroke = brushAxis;
				candleLine.StrokeThickness = 1;
				candleLine.X1 = xMargin + x * xAxisBarStep;
				candleLine.Y1 = height - yMargin - candles.;
				candleLine.X2 = xMargin + x * xAxisBarStep;
				candleLine.Y2 = height - yMargin;
				canGraph.Children.Add(candleLine);

			}
			*/


			// Make some data sets.
			//Brush[] brushes = { Brushes.Red, Brushes.Green, Brushes.Blue };
			Brush brushes = new SolidColorBrush(Colors.Blue);

			PointCollection points = new PointCollection();

			if (candles != null)
			{
				//
				double x = xMargin;
				double y = height - yMargin;
				//double cy = -10.0f; //200.0f; //10900.0f;

				//
				double xScale = 30.0f;
				double yScale = 5000.0f;
				double yOffset = 5000.0f;
				int xt = 1;

				// Calculate Y-scale and Y-offset
				double deltaCandleHeight = candleMax - candleMin;
				double deltaViewHeight = viewHeight - 0.0f;
				double candleScale = deltaViewHeight / deltaCandleHeight;


				//
				foreach (var item in candles)
				{
					//y = height - yMargin + yOffset - item.openAsk * yScale;
					y = height - yMargin + candleScale * candleMin - item.openAsk * candleScale;
					//Debug.Print("{0}", item.openAsk);

					points.Add(new Point(x, y));
					//x++;
					x = xMargin + xt * xAxisBarStep;
					xt++;
				}
			}

			Polyline polyline = new Polyline();
			polyline.StrokeThickness = 1;
			polyline.Stroke = brushes; //Brushes.Blue;
			polyline.Points = points;

			canGraph.Children.Add(polyline);

		}

		private void DrawGraphOrig()
		{
			// Canvas
			double width = canGraph.Width;
			double height = canGraph.Height;

			// Text
			double textH = width - 30.0f;
			double textV = 20.0f;
			double textMargin = 9.0f;

			// Center point for data
			double centerX = 0.0f;
			double centerY = canGraph.Height - 20.0f;

			// Scaling of data
			double viewMinY = 0.0f;
			double viewMaxY = 2.0f; // 2.0f;
			int stepX = candles.Count();
			int stepY = 10;
			double scaleX = textH / (stepX - 1); //10.0f;
			double scaleY = 1.0f; //1.0f; // 200.0f; //10900.0f;
			double markWidth = 10.0f;

			// Make the X axis
			Line xAxis = new Line();
			SolidColorBrush brush = new SolidColorBrush(Colors.Black);
			xAxis.Stroke = brush;
			xAxis.StrokeThickness = 1;
			xAxis.X1 = textH;
			xAxis.Y1 = 0;
			xAxis.X2 = textH;
			xAxis.Y2 = height;
			canGraph.Children.Add(xAxis);

			// Make the Y axis marks & value
			for (int i = 1; i < stepY; i++)
			{
				double ty = centerY - (i * (centerY / stepY));

				Line txMark = new Line();
				txMark.Stroke = brush;
				txMark.StrokeThickness = 1;
				txMark.X1 = textH - markWidth / 2;
				txMark.Y1 = ty;
				txMark.X2 = textH + markWidth / 2;
				txMark.Y2 = ty;
				canGraph.Children.Add(txMark);

				// Y axis text
				TextBlock textBlock = new TextBlock();
				textBlock.Text = ((viewMaxY - viewMinY) / stepY * i).ToString();
				textBlock.Foreground = new SolidColorBrush(Colors.Black);
				Canvas.SetLeft(textBlock, textH + textMargin);
				Canvas.SetTop(textBlock, ty - textMargin);
				canGraph.Children.Add(textBlock);
			}






			// Make the Y axis
			Line yAxis = new Line();
			//SolidColorBrush brush = new SolidColorBrush(Colors.Black);
			yAxis.Stroke = brush;
			yAxis.StrokeThickness = 1;
			yAxis.X1 = 0;
			yAxis.Y1 = centerY;
			yAxis.X2 = width;
			yAxis.Y2 = centerY;
			canGraph.Children.Add(yAxis);

			// Make some data sets.
			//Brush[] brushes = { Brushes.Red, Brushes.Green, Brushes.Blue };
			Brush brushes = new SolidColorBrush(Colors.Blue);

			PointCollection points = new PointCollection();

			if (candles != null)
			{
				double x = centerX;
				double y = centerY;
				//double cy = -10.0f; //200.0f; //10900.0f;
				foreach (var item in candles)
				{
					//x = x * xscale;
					y = centerY - (item.openAsk * scaleY) * centerY / 2;  //*140; // - centerY;
																		  //Debug.Print("{0}", item.openAsk);
					points.Add(new Point(x, y));
					//x++;
					x += 1.0f * scaleX;
				}
			}

			Polyline polyline = new Polyline();
			polyline.StrokeThickness = 1;
			polyline.Stroke = brushes; //Brushes.Blue;
			polyline.Points = points;

			canGraph.Children.Add(polyline);

		}


	}
}
