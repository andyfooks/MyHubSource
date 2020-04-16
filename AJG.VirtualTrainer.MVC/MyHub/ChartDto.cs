using AJG.VirtualTrainer.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AJG.VirtualTrainer.MVC.MyHub
{
    public enum ChartColumnDataType
    {
        none,
        financial,
    }
    public class ChartDTO
    {
        public ADUserDTO UserDTO { get; set; }
        public string type { get; set; }
        public ChartDataDto data { get; set; }
        public ChartOptionsDto options { get; set; }
        public bool showTable { get; set; }
        public bool showGraph { get; set; }
        public bool calculateRowTotals { get; set; }
        public ChartDTO(chartType chartType, List<string> dataLabels, List<ChartDataSetDto> dataSets, string chartTitle, 
            string leftYAxesName = "", string rightYAxesName = "", string xAxesName = "", bool showTable = true, bool showGraph = true, List<ChartColumnDataType> columnDataTypes = null, bool calculateRowTotals = true)
        {
            this.calculateRowTotals = calculateRowTotals;
            type = chartType.ToString();
            data = new ChartDataDto();
            options = new ChartOptionsDto();
            data.ColumnDataTypesInternal = columnDataTypes == null ? new List<ChartColumnDataType>() : columnDataTypes;
            data.labels.AddRange(dataLabels);
            data.datasets = dataSets;
            options.title.text = chartTitle;
            this.showTable = showTable;
            this.showGraph = showGraph;
            options.scales.xAxes.Add(new ChartAxesDto()
            {
                scaleLabel = new ChartscaleLableDto()
                {
                    display = string.IsNullOrEmpty(xAxesName) ? false : true,
                    labelString = string.IsNullOrEmpty(xAxesName) ? string.Empty : xAxesName
                }
            });
            if (dataSets.Where(d => d.yAxisID == whichYAxis.left.ToString()).Any())
            {
                options.scales.yAxes.Add(new ChartYAxesDto()
                {
                    id = whichYAxis.left.ToString(),
                    position = "left",
                    type = "linear",
                    scaleLabel = new ChartscaleLableDto()
                    {
                        display = string.IsNullOrEmpty(leftYAxesName) ? false : true,
                        labelString = string.IsNullOrEmpty(leftYAxesName) ? string.Empty : leftYAxesName,
                    }
                });
            }
            if (dataSets.Where(d => d.yAxisID == whichYAxis.right.ToString()).Any())
            {
                options.scales.yAxes.Add(new ChartYAxesDto()
                {
                    id = whichYAxis.right.ToString(),
                    position = "right",
                    type = "linear",
                    scaleLabel = new ChartscaleLableDto()
                    {
                        display = string.IsNullOrEmpty(rightYAxesName) ? false : true,
                        labelString = string.IsNullOrEmpty(rightYAxesName) ? string.Empty : rightYAxesName,
                    }
                });
            }
        }
        public class ChartDataDto
        {
            // E.g. x axes names e.g month or user name
            public List<string> labels { get; set; }
            internal List<ChartColumnDataType> ColumnDataTypesInternal { get; set; }
            private List<ChartDataSetDto> internalDatasets = new List<ChartDataSetDto>();
            private int startNumberRed = 0;
            private int startNumberGreen = 80;
            private int startNumberBlue = 160;
            public List<string> ColumnDataTypes
            {
                get
                {
                    var returnList = new List<string>();
                    foreach(var type in ColumnDataTypesInternal)
                    {
                        returnList.Add(type.ToString());
                    }
                    return returnList;
                }
            }

            public List<ChartDataSetDto> datasets
            {
                get
                {                    
                    List<string> colours = new List<string>();
                    for (int i = 0; i < internalDatasets.Count(); i++)
                    {
                        int addition = i * 70;
                        startNumberRed = startNumberRed + addition;
                        startNumberGreen = startNumberGreen + addition;
                        startNumberBlue = startNumberBlue + addition;
                        colours.Add(string.Format("rgb({0},{1},{2})", (startNumberRed % 240), (startNumberGreen % 240), (startNumberBlue % 240)));
                    }

                    for (int i = 0; i < internalDatasets.Count(); i++)
                    {
                        internalDatasets[i].borderColor = colours[i];
                    }                    

                    return internalDatasets;
                }
                set
                {
                    internalDatasets = value;
                }
            }
            public ChartDataDto()
            {
                labels = new List<string>();
                internalDatasets = new List<ChartDataSetDto>();
                ColumnDataTypesInternal = new List<ChartColumnDataType>();
            }
        }
        public class ChartOptionsDto
        {
            public ChartTitleDto title { get; set; }
            public ChartLegendDto legend { get; set; }
            public ChartScalesDto scales { get; set; }
            public ChartOptionsDto()
            {
                title = new ChartTitleDto();
                legend = new ChartLegendDto();
                scales = new ChartScalesDto();
            }
        }
        public class ChartDataSetDto
        {
            public string label { get; set; }
            public bool fill { get; set; }
            public string borderColor { get; set; }
            public List<decimal> data { get; set; }
            public double? lineTension { get; set; }
            public string yAxisID { get; set; }
            public bool IsFinancialData { get; set; }
            public string FontWeight { get; set; }
            public bool SetColor { get; set; }

            public ChartDataSetDto(List<decimal> data, string label, whichYAxis Yaxis, bool isFinancialData = false, string fontWeight = "normal", bool setColor = false)
            {
                lineTension = 0;
                fill = false;
                this.label = label;
                this.data = data;
                yAxisID = Yaxis.ToString();
                this.FontWeight = fontWeight;
                this.IsFinancialData = isFinancialData;
                this.SetColor = setColor;
            }
        }
        public class ChartTitleDto
        {
            public string position { get; set; }
            public bool display { get; set; }
            public string text { get; set; }
            public int? fontSize { get; set; }
            public ChartTitleDto()
            {
                position = "top";
                display = true;
                fontSize = 16;
            }
        }
        public class ChartLegendDto
        {
            public bool display { get; set; }
            public string position { get; set; }
            public ChartLabelDto labels { get; set; }
            public ChartLegendDto()
            {
                display = true;
                labels = new ChartLabelDto();
                position = "bottom";
            }
        }
        public class ChartLabelDto
        {
            public string fontColor { get; set; }
            public ChartLabelDto()
            {
                fontColor = string.Format("rgb({0},{1},{2})", 255, 99, 132);
            }
        }
        public class ChartScalesDto
        {
            public List<ChartYAxesDto> yAxes { get; set; }
            public List<ChartAxesDto> xAxes { get; set; }
            public ChartScalesDto()
            {
                yAxes = new List<ChartYAxesDto>();
                xAxes = new List<ChartAxesDto>();
            }
        }
        public class ChartYAxesDto : ChartAxesDto
        {
            public string id { get; set; }
            public string type { get; set; }
            public string position { get; set; }
            public ChartYAxesDto()
            {
                type = "linear";
                position = "left";
                scaleLabel = new ChartscaleLableDto();
            }
        }
        public class ChartAxesDto
        {
            public ChartscaleLableDto scaleLabel { get; set; }
            public ChartAxesDto()
            {
                scaleLabel = new ChartscaleLableDto();
            }
        }
        public class ChartscaleLableDto
        {
            public bool display { get; set; }
            public string labelString { get; set; }
            public ChartscaleLableDto()
            {
                display = true;
                labelString = string.Empty;
            }
        }
    }
    public enum chartType
    {
        bar,
        line
    }
    public enum whichYAxis
    {
        left,
        right
    }
}