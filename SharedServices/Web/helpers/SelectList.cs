using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SharedServices.Web.Helpers
{
    internal class SelectListItem<TModel>:SelectListItem
    {
        public TModel Model { get; private set; }
        public string GroupValue { get; private set; }
        internal SelectListItem(TModel model, Func<TModel, object> valueSelector, Func<TModel, object> textSelector)
        {
            Model = model;
            Value = Model.GetSelectorValue(valueSelector).ToString();
            Text = Model.GetSelectorValue(textSelector).ToString();
        }
        internal SelectListItem(TModel model, Func<TModel, object> valueSelector, Func<TModel, object> textSelector, Func<TModel, object> groupSelector):this(model, valueSelector, textSelector)
        {
            GroupValue = Model.GetSelectorValue(groupSelector).ToString();
        }
    }
    internal class MultiSelectList<TModel> : MultiSelectList, IEnumerable<SelectListItem<TModel>>
    {
        private IEnumerable<SelectListItem<TModel>> FormattedItems { get=> Items as IEnumerable<SelectListItem<TModel>>; }
        IEnumerator<SelectListItem<TModel>> IEnumerable<SelectListItem<TModel>>.GetEnumerator() => FormattedItems.GetEnumerator();

        private static string ValueField = "Value";
        private static string TextField = "Text";
        private static string GroupByField = "GroupValue";

        internal MultiSelectList(IEnumerable<TModel> items, Func<TModel, object> valueSelector, Func<TModel, object> textSelector)
            : base(items.ToSelectListItemEnumerable(valueSelector, textSelector), ValueField, TextField) { }
        internal MultiSelectList(IEnumerable<TModel> items, Func<TModel, object> valueSelector, Func<TModel, object> textSelector, Func<TModel, bool> isSelected)
            : base(items.ToSelectListItemEnumerable(valueSelector, textSelector), ValueField, TextField, items.GetValueCheck(valueSelector, isSelected)) { }
        internal MultiSelectList(IEnumerable<TModel> items, Func<TModel, object> valueSelector, Func<TModel, object> textSelector, Func<TModel, bool> isSelected, Func<TModel, bool> isDisabled)
            : base(items.ToSelectListItemEnumerable(valueSelector, textSelector), ValueField, TextField, items.GetValueCheck(valueSelector, isSelected), items.GetValueCheck(valueSelector, isDisabled)) { }
        internal MultiSelectList(IEnumerable<TModel> items, Func<TModel, object> valueSelector, Func<TModel, bool> isSelected)
            : this(items,valueSelector, valueSelector, isSelected) { }
        internal MultiSelectList(IEnumerable<TModel> items, Func<TModel, object> valueSelector, Func<TModel, bool> isSelected, Func<TModel, bool> isDisabled)
            : this(items, valueSelector, valueSelector, isSelected, isDisabled) { }
        internal MultiSelectList(IEnumerable<TModel> items, Func<TModel, object> valueSelector, Func<TModel, object> textSelector, Func<TModel, object> groupSelector)
            : base(items.ToSelectListItemEnumerable(valueSelector, textSelector, groupSelector), ValueField, TextField, GroupByField) { }
        internal MultiSelectList(IEnumerable<TModel> items, Func<TModel, object> valueSelector, Func<TModel, object> textSelector, Func<TModel, object> groupSelector, Func<TModel, bool> isSelected)
            : base(items.ToSelectListItemEnumerable(valueSelector, textSelector, groupSelector), ValueField, TextField, GroupByField, items.GetValueCheck(valueSelector, isSelected)) { }
        internal MultiSelectList(IEnumerable<TModel> items, Func<TModel, object> valueSelector, Func<TModel, object> textSelector, Func<TModel, object> groupSelector, Func<TModel, bool> isSelected, Func<TModel, bool> isDisabled)
            : base(items.ToSelectListItemEnumerable(valueSelector, textSelector, groupSelector), ValueField, TextField, GroupByField, items.GetValueCheck(valueSelector, isSelected), items.GetValueCheck(valueSelector, isDisabled)) { }
    }
    internal class SelectList<TModel> : SelectList, IEnumerable<SelectListItem<TModel>>
    {
        private IEnumerable<SelectListItem<TModel>> FormattedItems { get => Items as IEnumerable<SelectListItem<TModel>>; }
        IEnumerator<SelectListItem<TModel>> IEnumerable<SelectListItem<TModel>>.GetEnumerator() => FormattedItems.GetEnumerator();

        private static string ValueField = "Value";
        private static string TextField = "Text";
        private static string GroupByField = "GroupValue";

        internal SelectList(IEnumerable<TModel> items, Func<TModel, object> valueSelector, Func<TModel, object> textSelector)
            : base(items.ToSelectListItemEnumerable(valueSelector, textSelector), ValueField, TextField) { }
        internal SelectList(IEnumerable<TModel> items, Func<TModel, object> valueSelector, Func<TModel, object> textSelector, Func<TModel, bool> isSelected)
            : base(items.ToSelectListItemEnumerable(valueSelector, textSelector), ValueField, TextField, items.GetValueCheckFirst(valueSelector, isSelected)) { }
        internal SelectList(IEnumerable<TModel> items, Func<TModel, object> valueSelector, Func<TModel, object> textSelector, Func<TModel, bool> isSelected, Func<TModel, bool> isDisabled)
            : base(items.ToSelectListItemEnumerable(valueSelector, textSelector), ValueField, TextField, items.GetValueCheckFirst(valueSelector, isSelected), items.GetValueCheck(valueSelector, isDisabled)) { }
        internal SelectList(IEnumerable<TModel> items, Func<TModel, object> valueSelector, Func<TModel, bool> isSelected)
            : this(items, valueSelector, valueSelector, isSelected) { }
        internal SelectList(IEnumerable<TModel> items, Func<TModel, object> valueSelector, Func<TModel, bool> isSelected, Func<TModel, bool> isDisabled)
            : this(items, valueSelector, valueSelector, isSelected, isDisabled) { }
        internal SelectList(IEnumerable<TModel> items, Func<TModel, object> valueSelector, Func<TModel, object> textSelector, Func<TModel, object> groupSelector)
            : base(items.ToSelectListItemEnumerable(valueSelector, textSelector, groupSelector), ValueField, TextField, GroupByField) { }
        internal SelectList(IEnumerable<TModel> items, Func<TModel, object> valueSelector, Func<TModel, object> textSelector, Func<TModel, object> groupSelector, Func<TModel, bool> isSelected)
            : base(items.ToSelectListItemEnumerable(valueSelector, textSelector, groupSelector), ValueField, TextField, GroupByField, items.GetValueCheckFirst(valueSelector, isSelected)) { }
        internal SelectList(IEnumerable<TModel> items, Func<TModel, object> valueSelector, Func<TModel, object> textSelector, Func<TModel, object> groupSelector, Func<TModel, bool> isSelected, Func<TModel, bool> isDisabled)
            : base(items.ToSelectListItemEnumerable(valueSelector, textSelector, groupSelector), ValueField, TextField, GroupByField, items.GetValueCheckFirst(valueSelector, isSelected), items.GetValueCheck(valueSelector, isDisabled)) { }
    }
    public static class SelectListExtensions
    {
        internal static IEnumerable<SelectListItem<TModel>> ToSelectListItemEnumerable<TModel>(this IEnumerable<TModel> models, Func<TModel, object> valueSelector, Func<TModel, object> textSelector) => models.Select(m => new SelectListItem<TModel>(m,valueSelector, textSelector));
        internal static IEnumerable<SelectListItem<TModel>> ToSelectListItemEnumerable<TModel>(this IEnumerable<TModel> models, Func<TModel, object> valueSelector, Func<TModel, object> textSelector, Func<TModel, object> groupSelector) => models.Select(m => new SelectListItem<TModel>(m,valueSelector, textSelector, groupSelector));
        internal static  TVal GetSelectorValue<TModel,TVal>(this TModel model, Func<TModel, TVal> selector, TVal defaultValue = default)
        {
            try
            {
                TVal value = selector(model);
                return value != null ? value : defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        internal static IEnumerable<string> GetValueCheck<TModel>(this IEnumerable<TModel>models, Func<TModel, object> valueSelector, Func<TModel, bool> checkSelector, bool defaultValue = false)
        => models.Where(m => m.GetSelectorValue(checkSelector, defaultValue)).Select(m => m.GetSelectorValue(valueSelector).ToString());
        internal static object GetValueCheckFirst<TModel>(this IEnumerable<TModel> models, Func<TModel, object> valueSelector, Func<TModel, bool> checkSelector, bool defaultValue = false)
        => models.GetValueCheck(valueSelector, checkSelector, defaultValue).FirstOrDefault();

        public static MultiSelectList ToMultiSelectList<TModel>(this IEnumerable<TModel> items, Func<TModel, object> valueSelector, Func<TModel, object> textSelector)
            => new MultiSelectList<TModel>(items, valueSelector, textSelector);
        public static MultiSelectList ToMultiSelectList<TModel>(this IEnumerable<TModel> items, Func<TModel, object> valueSelector, Func<TModel, object> textSelector, Func<TModel, bool> isSelected)
            => new MultiSelectList<TModel>(items, valueSelector, textSelector,isSelected);
        public static MultiSelectList ToMultiSelectList<TModel>(this IEnumerable<TModel> items, Func<TModel, object> valueSelector, Func<TModel, object> textSelector, Func<TModel, bool> isSelected, Func<TModel, bool> isDisabled)
            => new MultiSelectList<TModel>(items, valueSelector, textSelector,isSelected,isDisabled);
        public static MultiSelectList ToMultiSelectList<TModel>(this IEnumerable<TModel> items, Func<TModel, object> valueSelector, Func<TModel, bool> isSelected)
            => new MultiSelectList<TModel>(items, valueSelector, isSelected);
        public static MultiSelectList ToMultiSelectList<TModel>(this IEnumerable<TModel> items, Func<TModel, object> valueSelector, Func<TModel, bool> isSelected, Func<TModel, bool> isDisabled)
            => new MultiSelectList<TModel>(items, valueSelector, isSelected, isDisabled);
        public static MultiSelectList ToMultiSelectList<TModel>(this IEnumerable<TModel> items, Func<TModel, object> valueSelector, Func<TModel, object> textSelector, Func<TModel, object> groupSelector)
            => new MultiSelectList<TModel>(items, valueSelector, textSelector,groupSelector);
        public static MultiSelectList ToMultiSelectList<TModel>(this IEnumerable<TModel> items, Func<TModel, object> valueSelector, Func<TModel, object> textSelector, Func<TModel, object> groupSelector, Func<TModel, bool> isSelected)
            => new MultiSelectList<TModel>(items, valueSelector, textSelector, groupSelector, isSelected);
        public static MultiSelectList ToMultiSelectList<TModel>(this IEnumerable<TModel> items, Func<TModel, object> valueSelector, Func<TModel, object> textSelector, Func<TModel, object> groupSelector, Func<TModel, bool> isSelected, Func<TModel, bool> isDisabled)
            => new MultiSelectList<TModel>(items, valueSelector, textSelector, groupSelector, isSelected, isDisabled);
        public static SelectList ToSelectList<TModel>(this IEnumerable<TModel> items, Func<TModel, object> valueSelector, Func<TModel, object> textSelector)
            => new SelectList<TModel>(items, valueSelector, textSelector);
        public static SelectList ToSelectList<TModel>(this IEnumerable<TModel> items, Func<TModel, object> valueSelector, Func<TModel, object> textSelector, Func<TModel, bool> isSelected)
            => new SelectList<TModel>(items, valueSelector, textSelector, isSelected);
        public static SelectList ToSelectList<TModel>(this IEnumerable<TModel> items, Func<TModel, object> valueSelector, Func<TModel, object> textSelector, Func<TModel, bool> isSelected, Func<TModel, bool> isDisabled)
            => new SelectList<TModel>(items, valueSelector, textSelector, isSelected, isDisabled);
        public static SelectList ToSelectList<TModel>(this IEnumerable<TModel> items, Func<TModel, object> valueSelector, Func<TModel, bool> isSelected)
            => new SelectList<TModel>(items, valueSelector, isSelected);
        public static SelectList ToSelectList<TModel>(this IEnumerable<TModel> items, Func<TModel, object> valueSelector, Func<TModel, bool> isSelected, Func<TModel, bool> isDisabled)
            => new SelectList<TModel>(items, valueSelector, isSelected, isDisabled);
        public static SelectList ToSelectList<TModel>(this IEnumerable<TModel> items, Func<TModel, object> valueSelector, Func<TModel, object> textSelector, Func<TModel, object> groupSelector)
            => new SelectList<TModel>(items, valueSelector, textSelector, groupSelector);
        public static SelectList ToSelectList<TModel>(this IEnumerable<TModel> items, Func<TModel, object> valueSelector, Func<TModel, object> textSelector, Func<TModel, object> groupSelector, Func<TModel, bool> isSelected)
            => new SelectList<TModel>(items, valueSelector, textSelector, groupSelector, isSelected);
        public static SelectList ToSelectList<TModel>(this IEnumerable<TModel> items, Func<TModel, object> valueSelector, Func<TModel, object> textSelector, Func<TModel, object> groupSelector, Func<TModel, bool> isSelected, Func<TModel, bool> isDisabled)
            => new SelectList<TModel>(items, valueSelector, textSelector, groupSelector, isSelected, isDisabled);
    }

}