﻿using Horizon.MvvmFramework.Commands;
using SimpleInjector;
using System;
using Horizon.MvvmFramework.Services;
using Trackify.DataModels;
using Trackify.ViewModels;

namespace Trackify.Factories
{
    internal class ViewModelFactory : IViewModelFactory
    {
        private readonly Container _iocContainer;

        public ViewModelFactory(Container iocContainer)
        {
            _iocContainer = iocContainer;
        }

        public EditTimeAcquisitionViewModel CreateEditTimeAcquisitionViewModel(TimeAcquisitionModel timeAcquisition)
        {
            return new EditTimeAcquisitionViewModel(
                timeAcquisition,
                _iocContainer.GetInstance<IDatabaseContext>(),
                _iocContainer.GetInstance<IMessenger>(),
                _iocContainer.GetInstance<ICommandFactory>());
        }

        public SettingsViewModel CreateSettingsViewModel()
        { 
            return _iocContainer.GetInstance<SettingsViewModel>();
        }
    }
}