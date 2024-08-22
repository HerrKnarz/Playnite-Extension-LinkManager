﻿using KNARZhelper;
using KNARZhelper.Enum;
using MetadataUtilities.Models;
using MetadataUtilities.Views;
using Playnite.SDK;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using KNARZhelper.DatabaseObjectTypes;
using Action = MetadataUtilities.Models.Action;
using Condition = MetadataUtilities.Models.Condition;

namespace MetadataUtilities.ViewModels
{
    public class ConditionalActionEditorViewModel : ObservableObject
    {
        private readonly List<IDatabaseObjectType> _fieldTypes = FieldTypeHelper.GetAllTypes();
        private readonly Settings _settings;
        private ConditionalAction _conditionalAction;

        public ConditionalActionEditorViewModel(Settings settings, ConditionalAction conditionalAction)
        {
            _settings = settings;
            _conditionalAction = conditionalAction;

            //TODO: remove checks on Value Type once the contains buttons support adding other values that items!

            ContextMenuActionsAdd.AddMissing(_fieldTypes.Where(x => x.CanBeSetInGame && x.ValueType == ItemValueType.ItemList)
                .Select(x =>
                    new FieldTypeContextAction
                    {
                        Name = x.LabelSingular,
                        Action = AddActionAddCommand,
                        FieldType = x.Type
                    }
                ));

            ContextMenuActionsRemove.AddMissing(_fieldTypes.Where(x => x.CanBeSetInGame && x.CanBeEmptyInGame && x.ValueType == ItemValueType.ItemList)
                .Select(x =>
                    new FieldTypeContextAction
                    {
                        Name = x.LabelSingular,
                        Action = AddActionRemoveCommand,
                        FieldType = x.Type
                    }
                ));

            ContextMenuActionsClear.AddMissing(_fieldTypes.Where(x => x.CanBeSetInGame && x.CanBeEmptyInGame)
                .Select(x =>
                    new FieldTypeContextAction
                    {
                        Name = x.LabelSingular,
                        Action = AddActionClearCommand,
                        FieldType = x.Type
                    }
                ));

            ContextMenuConditionsContains.AddMissing(_fieldTypes.Where(x => x.ValueType == ItemValueType.ItemList && x.ValueType == ItemValueType.ItemList)
                .Select(x =>
                    new FieldTypeContextAction
                    {
                        Name = x.LabelSingular,
                        Action = AddConditionContainsCommand,
                        FieldType = x.Type
                    }
                ));

            ContextMenuConditionsContainsNot.AddMissing(_fieldTypes.Where(x => x.ValueType == ItemValueType.ItemList && x.ValueType == ItemValueType.ItemList)
                .Select(x =>
                    new FieldTypeContextAction
                    {
                        Name = x.LabelSingular,
                        Action = AddConditionContainsNotCommand,
                        FieldType = x.Type
                    }
                ));

            ContextMenuConditionsEmpty.AddMissing(_fieldTypes.Where(x => x.CanBeEmptyInGame)
                .Select(x =>
                    new FieldTypeContextAction
                    {
                        Name = x.LabelSingular,
                        Action = AddConditionIsEmptyCommand,
                        FieldType = x.Type
                    }
                ));
        }

        public RelayCommand<FieldType> AddActionAddCommand => new RelayCommand<FieldType>(type =>
            AddActions(type, ActionType.AddObject));

        public RelayCommand<FieldType> AddActionClearCommand => new RelayCommand<FieldType>(type =>
            AddActions(type, ActionType.ClearField));

        public RelayCommand<FieldType> AddActionRemoveCommand => new RelayCommand<FieldType>(type =>
                    AddActions(type, ActionType.RemoveObject));

        public RelayCommand<FieldType> AddConditionContainsCommand => new RelayCommand<FieldType>(type =>
            AddConditions(type, ComparatorType.Contains));

        public RelayCommand<FieldType> AddConditionContainsNotCommand => new RelayCommand<FieldType>(type =>
            AddConditions(type, ComparatorType.DoesNotContain));

        public RelayCommand<FieldType> AddConditionIsEmptyCommand => new RelayCommand<FieldType>(type =>
            AddConditions(type, ComparatorType.IsEmpty));

        public ConditionalAction ConditionalAction
        {
            get => _conditionalAction;
            set => SetValue(ref _conditionalAction, value);
        }

        public ObservableCollection<FieldTypeContextAction> ContextMenuActionsAdd { get; set; } =
            new ObservableCollection<FieldTypeContextAction>();

        public ObservableCollection<FieldTypeContextAction> ContextMenuActionsClear { get; set; } =
            new ObservableCollection<FieldTypeContextAction>();

        public ObservableCollection<FieldTypeContextAction> ContextMenuActionsRemove { get; set; } =
                    new ObservableCollection<FieldTypeContextAction>();

        public ObservableCollection<FieldTypeContextAction> ContextMenuConditionsContains { get; set; } =
            new ObservableCollection<FieldTypeContextAction>();

        public ObservableCollection<FieldTypeContextAction> ContextMenuConditionsContainsNot { get; set; } =
            new ObservableCollection<FieldTypeContextAction>();

        public ObservableCollection<FieldTypeContextAction> ContextMenuConditionsEmpty { get; set; } =
            new ObservableCollection<FieldTypeContextAction>();

        public RelayCommand<IList<object>> RemoveActionCommand => new RelayCommand<IList<object>>(items =>
        {
            foreach (Action item in items.ToList().Cast<Action>())
            {
                ConditionalAction.Actions.Remove(item);
            }
        }, items => items?.Count != 0);

        public RelayCommand<IList<object>> RemoveConditionCommand => new RelayCommand<IList<object>>(items =>
        {
            foreach (Condition item in items.ToList().Cast<Condition>())
            {
                ConditionalAction.Conditions.Remove(item);
            }
        }, items => items?.Count != 0);

        public RelayCommand<Window> SaveCommand => new RelayCommand<Window>(win =>
        {
            if (ConditionalAction.Name == null || ConditionalAction.Name?.Length == 0)
            {
                API.Instance.Dialogs.ShowMessage(ResourceProvider.GetString("LOCMetadataUtilitiesDialogNoNameSet"), string.Empty, MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (!ConditionalAction.Actions.Any())
            {
                API.Instance.Dialogs.ShowMessage(ResourceProvider.GetString("LOCMetadataUtilitiesDialogNoActionsSet"), string.Empty, MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (ConditionalAction.Enabled && !ConditionalAction.Conditions.Any())
            {
                if (API.Instance.Dialogs.ShowMessage(ResourceProvider.GetString("LOCMetadataUtilitiesDialogNoConditionsSet"), string.Empty, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                {
                    return;
                }
            }

            win.DialogResult = true;
            win.Close();
        });

        public static Window GetWindow(Settings settings, ConditionalAction conditionalAction)
        {
            try
            {
                ConditionalActionEditorViewModel viewModel = new ConditionalActionEditorViewModel(settings, conditionalAction);

                ConditionalActionEditorView conditionalActionEditorView = new ConditionalActionEditorView();

                Window window = WindowHelper.CreateSizedWindow(ResourceProvider.GetString("LOCMetadataUtilitiesDialogConditionalActionEditor"), 800, 500);
                window.Content = conditionalActionEditorView;
                window.DataContext = viewModel;

                return window;
            }
            catch (Exception exception)
            {
                Log.Error(exception, "Error during initializing edit conditional action dialog", true);

                return null;
            }
        }

        public void AddActions(FieldType fieldType, ActionType actionType)
        {
            if (actionType == ActionType.ClearField)
            {
                if (!ConditionalAction.Actions.Any(x => x.ActionType == actionType && x.Type == fieldType))
                {
                    ConditionalAction.Actions.Add(new Action(_settings)
                    {
                        Name = string.Empty,
                        Type = fieldType,
                        ActionType = actionType
                    });
                }

                return;
            }

            List<MetadataObject> items = MetadataFunctions.GetItemsFromAddDialog(fieldType, _settings);

            if (items.Count == 0)
            {
                return;
            }

            foreach (MetadataObject item in items.Where(item => ConditionalAction.Actions.All(x => x.TypeAndName != item.TypeAndName || x.ActionType != actionType)))
            {
                ConditionalAction.Actions.Add(new Action(_settings)
                {
                    Name = item.Name,
                    Type = item.Type,
                    ActionType = actionType
                });
            }

            ConditionalAction.Conditions = ConditionalAction.Conditions.OrderBy(x => x.ToString).ToObservable();
        }

        public void AddConditions(FieldType fieldType, ComparatorType comparatorType)
        {
            if (comparatorType == ComparatorType.IsEmpty)
            {
                if (!ConditionalAction.Conditions.Any(x => x.Comparator == comparatorType && x.Type == fieldType))
                {
                    ConditionalAction.Conditions.Add(new Condition(_settings)
                    {
                        Name = string.Empty,
                        Type = fieldType,
                        Comparator = comparatorType
                    });
                }

                return;
            }

            List<MetadataObject> items = MetadataFunctions.GetItemsFromAddDialog(fieldType, _settings);

            if (items.Count == 0)
            {
                return;
            }

            foreach (MetadataObject item in items.Where(item => ConditionalAction.Conditions.All(x => x.TypeAndName != item.TypeAndName || x.Comparator != comparatorType)))
            {
                ConditionalAction.Conditions.Add(new Condition(_settings)
                {
                    Name = item.Name,
                    Type = item.Type,
                    Comparator = comparatorType
                });
            }

            ConditionalAction.Conditions = ConditionalAction.Conditions.OrderBy(x => x.ToString).ToObservable();
        }
    }
}