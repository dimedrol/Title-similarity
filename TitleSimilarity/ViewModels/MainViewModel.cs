using Data;
using Providers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace TitleSimilarity
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string _DefaultLocation = Location.Espoo.ToString( );
        private readonly Dispatcher _UIDispatcher;
        public MainViewModel( )
        {
            _UIDispatcher = Dispatcher.CurrentDispatcher;
            _LocationSourceList = new ObservableCollection<string>( );
            ApplyRadiusCommand = new Command( OnApplyRadiusCommandExecuted , OnApplyRadiusCommandCanExecute );

            RefreshView( );
        }



        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged( [CallerMemberName] String propertyName = "" )
        {
            PropertyChanged?.Invoke( this , new PropertyChangedEventArgs( propertyName ) );
        }

        public void RefreshCommands( )
        {
            try
            {
                Invoke( CommandManager.InvalidateRequerySuggested );
            }
            catch
            {
                CommandManager.InvalidateRequerySuggested( );
            }
        }

        public void Invoke( Action action , params object[ ] args )
        {
            _UIDispatcher.Invoke( action , args );
        }

        public ICommand ApplyRadiusCommand { get; set; }
        private void OnApplyRadiusCommandExecuted( object parms )
        {
            RefreshView( );
        }

        private bool OnApplyRadiusCommandCanExecute( object parms )
        {
            if( IsLoadingTitles )
                return false;

            if( IsProcessingTitles )
                return false;

            if( !IsRadiusChanged )
                return false;

            return true;
        }


        private ObservableCollection<string> _LocationSourceList;
        public ObservableCollection<string> LocationSourceList
        {
            get
            {
                return _LocationSourceList;
            }
            set
            {
                if( _LocationSourceList != value )
                {
                    _LocationSourceList = value;
                    OnPropertyChanged( );
                }
            }
        }


        private string _Radius = "10000";
        public string Radius
        {
            get
            {
                return _Radius;
            }
            set
            {
                if( _Radius != value )
                {
                    _Radius = value;
                    OnPropertyChanged( );
                    IsRadiusChanged = true;
                }
            }
        }

        private string _SelectedLocation;
        public string SelectedLocation
        {
            get
            {
                return _SelectedLocation;
            }
            set
            {
                if( _SelectedLocation != value )
                {
                    _SelectedLocation = value;
                    OnPropertyChanged( );
                    LoadSummary( );
                }
            }
        }

        private bool _IsRadiusChanged;
        public bool IsRadiusChanged
        {
            get
            {
                return _IsRadiusChanged;
            }
            set
            {
                if( _IsRadiusChanged != value )
                {
                    _IsRadiusChanged = value;
                    OnPropertyChanged( );
                    RefreshCommands( );
                }
            }
        }


        private bool _IsLoadingTitles;
        public bool IsLoadingTitles
        {
            get
            {
                return _IsLoadingTitles;
            }
            set
            {
                if( _IsLoadingTitles != value )
                {
                    _IsLoadingTitles = value;
                    OnPropertyChanged( );
                }
            }
        }

        private string _Summary;
        public string Summary
        {
            get
            {
                return _Summary;
            }
            set
            {
                if( _Summary != value )
                {
                    _Summary = value;
                    OnPropertyChanged( );
                }
            }
        }

        private bool _IsProcessingTitles;
        public bool IsProcessingTitles
        {
            get
            {
                return _IsProcessingTitles;
            }
            set
            {
                if( _IsProcessingTitles != value )
                {
                    _IsProcessingTitles = value;
                    OnPropertyChanged( );
                }
            }
        }

        private bool _IsBusy;
        public bool IsBusy
        {
            get
            {
                return _IsBusy;
            }
            set
            {
                if( _IsBusy != value )
                {
                    _IsBusy = value;
                    OnPropertyChanged( );
                }
            }
        }


        private void RefreshView( )
        {
            var locationList = LocationProvider.GetLocations( );
            locationList.ForEach( location =>
            {
                LocationSourceList.Add( location );

                if( location.Equals( _DefaultLocation ) )
                    SelectedLocation = location;
            } );

            LoadSummary( );
        }

        private void LoadSummary( )
        {
            if( string.IsNullOrEmpty( _SelectedLocation ) )
                return;

            Enum.TryParse<Location>( _SelectedLocation , out var location );

            if( !int.TryParse( _Radius , out var radius ) )
            {
                MessageBox.Show( "Please enter correct radius." , "Incorrect radius" , MessageBoxButton.OK , MessageBoxImage.Error );
                return;
            }

            if( radius > 10000 )
            {
                MessageBox.Show( "Radius may not be over 10000. min: 10, max: 10000." , "Incorrect radius" , MessageBoxButton.OK , MessageBoxImage.Error );
                return;
            }

            Task.Factory.StartNew( ( ) =>
            {
                Summary = string.Empty;
                IsLoadingTitles = true;
                IsBusy = true;


                var provider = new GeosearchProvider( );

                return provider.GetImageTitles( location , radius );
            } ).ContinueWith( t =>
            {
                IsLoadingTitles = false;
                IsProcessingTitles = true;

                var result = new List<KeyValuePair<string , double>>( );
                if( t.IsFaulted )
                {
                    Invoke( ( ) => MessageBox.Show( $"An error occurred while attempting to get image titles for selected location '{_SelectedLocation}'" , "Error" , MessageBoxButton.OK , MessageBoxImage.Error ) );
                }
                else
                {
                    result = SimilarityProvider.ProcessStrings( t.Result );
                }
                
                return result;

            } ).ContinueWith( t =>
            {
                IsProcessingTitles = false;

                var sb = new StringBuilder( );

                t.Result.ForEach( title =>
                {
                    var similarityPercent = Math.Round( title.Value * 100 , 2 );
                    sb.AppendLine( $"{title.Key} : {similarityPercent}%" );
                } );

                Summary = sb.ToString( );

                IsProcessingTitles = false;
                IsBusy = false;
            } );
        }
    }
}
