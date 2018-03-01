/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       WordCloud.cs
 *  Namespace:  Ewav    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    27/05/2014    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.ServiceModel;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Media;
//       using ItsCodingTime.Utils.Silverlight;  
//                                 using TagCloud.TagCloudControl.TagCloudService;    

namespace Ewav
{

    public class WordCloud
    {
        private ObservableCollection<WordCloudWord> cloudWords;

        public string BaseUrl { get; private set; }
        public int TagThreshold { get; private set; }
        //public FontFamily TagFontFamily { get; private set; }
        public int MinimumFontSize  =  7    ;    //               { get; private set; }
        //public SolidColorBrush TagBrush { get; private set; }
        //public SolidColorBrush TagColorHoverBrush { get; private set; }

        private const int _defaultTagThreshold = 2;
        private const int _defaultFontSize = 12;
        private Brush _tagItemOriginalBrush;

        //private readonly FontFamily _defaultFontFamily = new FontFamily("Arial");
        //private readonly SolidColorBrush _defaultTagItemBrush = new SolidColorBrush(Colors.Blue);
        //private readonly SolidColorBrush _defaultTagItemHoverBrush = new SolidColorBrush(Colors.Purple);
        //private readonly SolidColorBrush _defaultBackGroundBrush = new SolidColorBrush(Colors.White);
        //private string _tagCloudControlClientId;

        private bool _tagsLoaded;
        //                       private TagCloudServiceClient _tagCloudService;
        private ObservableCollection<WordCloudWord> _cloudTagCollection;

        public System.Windows.Controls.WrapPanel wrapPanel;

        //public WordCloud(string tagCloudControlClientId, string tagThreshold, string fontFamily, string minimumFontSize, string tagColor,
        //    string tagHoverColor, string backgroundColor)
        //{
        //   //          InitializeComponent();

        //    //  HtmlPage.RegisterScriptableObject("JStoSLBridge", this);	// register our "javascript to silverlight" bridge

        //    IntializeControl(tagCloudControlClientId, tagThreshold, fontFamily, minimumFontSize, tagColor, tagHoverColor, backgroundColor);

        //    InitializeWCFService();
        //}

        /// <summary>
        /// Initializes a new instance of the <see cref="WordCloud" /> class.
        /// </summary>
        //public WordCloud()
        //{
        //}


        public WordCloud(WrapPanel wrapPanel, ObservableCollection<WordCloudWord> cloudWords)
        {

            this.wrapPanel = wrapPanel;
            this._cloudTagCollection = cloudWords;
            tagCloudService_GetTagsCompleted(cloudWords);

        }


        //private void IntializeControl(string tagCloudControlClientId, string tagThreshold, string fontFamily, string minimumFontSize, string tagColor,
        //    string tagHoverColor, string backgroundColor)
        //{
        //    if (!string.IsNullOrEmpty(tagCloudControlClientId))
        //    {
        //        try
        //        {
        //            _tagCloudControlClientId = tagCloudControlClientId;
        //        }
        //        catch (Exception)
        //        {
        //            return;	// can't do much w/out the control's client id
        //        }
        //    }
        //    else
        //    {
        //        return;	// can't do much w/out the control's client id
        //    }

        //    if (!string.IsNullOrEmpty(tagThreshold))
        //    {
        //        try
        //        {
        //            TagThreshold = Convert.ToInt32(tagThreshold);
        //        }
        //        catch (Exception)
        //        {
        //            TagThreshold = _defaultTagThreshold;
        //        }
        //    }
        //    else
        //    {
        //        TagThreshold = _defaultTagThreshold;
        //    }

        //    if (!string.IsNullOrEmpty(fontFamily))
        //    {
        //        try
        //        {
        //            TagFontFamily = new FontFamily(fontFamily);
        //        }
        //        catch (Exception)
        //        {
        //            TagFontFamily = _defaultFontFamily;
        //        }
        //    }
        //    else
        //    {
        //        TagFontFamily = _defaultFontFamily;
        //    }

        //    if (!string.IsNullOrEmpty(tagColor))
        //    {
        //        try
        //        {
        //            //    var colorName = new ColorName(tagColor);
        //            //   TagBrush = new SolidColorBrush(colorName);
        //        }
        //        catch (Exception)
        //        {
        //            TagBrush = _defaultTagItemBrush;
        //        }
        //    }
        //    else
        //    {
        //        TagBrush = _defaultTagItemBrush;
        //    }

        //    if (!string.IsNullOrEmpty(tagHoverColor))
        //    {
        //        try
        //        {
        //            //    var colorName = new ColorName(tagHoverColor);
        //            //   TagColorHoverBrush = new SolidColorBrush(colorName);
        //        }
        //        catch (Exception)
        //        {
        //            TagColorHoverBrush = _defaultTagItemHoverBrush;
        //        }
        //    }
        //    else
        //    {
        //        TagColorHoverBrush = _defaultTagItemHoverBrush;
        //    }

        //    if (!string.IsNullOrEmpty(backgroundColor))
        //    {
        //        try
        //        {
        //            //    var colorName = new ColorName(backgroundColor);
        //            //              wrapPanel.Background = new SolidColorBrush(colorName);
        //        }
        //        catch (Exception)
        //        {
        //            wrapPanel.Background = _defaultBackGroundBrush;
        //        }
        //    }
        //    else
        //    {
        //        wrapPanel.Background = _defaultBackGroundBrush;
        //    }

        //    if (!string.IsNullOrEmpty(minimumFontSize))
        //    {
        //        try
        //        {
        //            MinimumFontSize = Convert.ToInt32(minimumFontSize);
        //        }
        //        catch (Exception)
        //        {
        //            MinimumFontSize = _defaultFontSize;
        //        }
        //    }
        //    else
        //    {
        //        MinimumFontSize = _defaultFontSize;
        //    }

        //    return;
        //}

        private int GetTagMaxOccurrences()
        {
            var maxOccurrences = 0;

            foreach (var tagCloud in _cloudTagCollection)
            {
                var tagOccurrences = tagCloud.TagOccurrences;

                if (tagOccurrences > maxOccurrences)
                {
                    maxOccurrences = tagOccurrences;
                }
            }

            return (maxOccurrences);
        }

        void tagCloudService_GetTagsCompleted(ObservableCollection<WordCloudWord> cloudWords)
        {
            if (cloudWords.Count == 0)
            {
                return;
            }

            _cloudTagCollection = cloudWords;

            var maxOccurrences = GetTagMaxOccurrences();

            if (maxOccurrences == 0)
            {
                return;
            }

            foreach (var cloudTag in _cloudTagCollection)
            {
                if (cloudTag.Equals(null))
                    continue;

                if (cloudTag.TagOccurrences < TagThreshold)	// if tag occurrence is below threshold, do not add; the service shouldn't return these, but just in case
                {
                    continue;
                }

                // verify the taglink url is good
                //if (!Validator.IsUrlFormatValid(cloudTag.TagLink))
                //    continue;

                // we'll wrap a TextBlock in the HyperlinkButton in order to support text wrapping
                var textBlock = new TextBlock { Text = cloudTag.TagName, TextWrapping = TextWrapping.Wrap };

                var lnkBtn = new HyperlinkButton
                {
                    Content = textBlock,
                    FontSize = CalcTagFontSize(cloudTag.TagOccurrences, maxOccurrences),
                  //    NavigateUri = new Uri(cloudTag.TagLink),
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center,
                   //  FontFamily = TagFontFamily,
                   //         Foreground = TagBrush
                };

                //lnkBtn.MouseEnter += TagCloudItem_MouseEnter;	// we'll do some styling on mouse over
                //lnkBtn.MouseLeave += TagCloudItem_MouseLeave;	// we'll restore styling on mouse leave

                wrapPanel.Children.Add(lnkBtn);
            }

            return;
            _tagsLoaded = true;

        }


        private double CalcTagFontSize(int tagOccurrences, int maxTagOccurrences)
        {
            if (maxTagOccurrences == 0)	// this probably shouldn't happen, but you never know...
            {
                return (MinimumFontSize);
            }

            var percent = 150 * (1.0 + ((1.5 * tagOccurrences) - (maxTagOccurrences / 2.0)) / maxTagOccurrences);
            var fontSize = (percent / 100) * MinimumFontSize;

            return (fontSize);
        }

        //private void WrapPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        //{
        //    if (HtmlPage.Document != null && _tagsLoaded)	// don't start resizing based on content until content loaded
        //    {
        //        var silverlightObjectTag = HtmlPage.Document.GetElementById(_tagCloudControlClientId);
        //        var silverlightControlHost = silverlightObjectTag.Parent;

        //        if (silverlightObjectTag != null)
        //        {
        //            silverlightObjectTag.SetStyleAttribute("height", e.NewSize.Height + "px");
        //        }

        //        if (silverlightControlHost != null)
        //        {
        //            silverlightControlHost.SetStyleAttribute("height", e.NewSize.Height + "px");
        //        }
        //    }

        //    return;
        //}



    }

    public class WordCloudWord : object, System.ComponentModel.INotifyPropertyChanged
    {

        private string TagLinkField;

        private string TagNameField;

        private int TagOccurrencesField;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string TagLink
        {
            get
            {
                return this.TagLinkField;
            }
            set
            {
                if ((object.ReferenceEquals(this.TagLinkField, value) != true))
                {
                    this.TagLinkField = value;
                    this.RaisePropertyChanged("TagLink");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string TagName
        {
            get
            {
                return this.TagNameField;
            }
            set
            {
                if ((object.ReferenceEquals(this.TagNameField, value) != true))
                {
                    this.TagNameField = value;
                    this.RaisePropertyChanged("TagName");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public int TagOccurrences
        {
            get
            {
                return this.TagOccurrencesField;
            }
            set
            {
                if ((this.TagOccurrencesField.Equals(value) != true))
                {
                    this.TagOccurrencesField = value;
                    this.RaisePropertyChanged("TagOccurrences");
                }
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }



}