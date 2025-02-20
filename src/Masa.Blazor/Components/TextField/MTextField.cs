﻿using BlazorComponent;
using BlazorComponent.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Masa.Blazor
{
    public partial class MTextField<TValue> : MInput<TValue>, ITextField<TValue>
    {
        private string _badInput;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _shouldRender = true;

        [Parameter]
        public virtual bool Clearable { get; set; }

        [Parameter]
        public bool PersistentPlaceholder { get; set; }

        [Parameter]
        public string ClearIcon { get; set; } = "mdi-close";

        [Parameter]
        public bool FullWidth { get; set; }

        [Parameter]
        public string Prefix { get; set; }

        [Parameter]
        public bool SingleLine { get; set; }

        [Parameter]
        public bool Solo { get; set; }

        [Parameter]
        public bool SoloInverted { get; set; }

        [Parameter]
        public bool Flat { get; set; }

        [Parameter]
        public bool Filled { get; set; }

        [Parameter]
        public virtual bool Outlined { get; set; }

        [Parameter]
        public bool Reverse { get; set; }

        [Parameter]
        public string Placeholder { get; set; }

        [Parameter]
        public bool Rounded { get; set; }

        [Parameter]
        public bool Shaped { get; set; }

        [Parameter]
        public string Type { get; set; } = "text";

        [Parameter]
        public RenderFragment PrependInnerContent { get; set; }

        [Parameter]
        public string AppendOuterIcon { get; set; }

        [Parameter]
        public RenderFragment AppendOuterContent { get; set; }

        [Parameter]
        public string PrependInnerIcon { get; set; }

        [Parameter]
        public string Suffix { get; set; }

        [Parameter]
        public bool Autofocus { get; set; }

        [Parameter]
        public EventCallback<FocusEventArgs> OnBlur { get; set; }

        [Parameter]
        public EventCallback<FocusEventArgs> OnFocus { get; set; }

        [Parameter]
        public EventCallback<KeyboardEventArgs> OnKeyDown { get; set; }

        [Parameter]
        public EventCallback<TValue> OnInput { get; set; }

        [Parameter]
        public RenderFragment ProgressContent { get; set; }

        [Parameter]
        public StringNumber LoaderHeight { get; set; } = 2;

        [Parameter]
        public StringNumberBoolean Counter { get; set; }

        [Parameter]
        public Func<TValue, int> CounterValue { get; set; }

        [Parameter]
        public RenderFragment CounterContent { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnAppendOuterClick { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnPrependInnerClick { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClearClick { get; set; }

        [Inject]
        public MasaBlazor MasaBlazor { get; set; }

        [Inject]
        public Document Document { get; set; }

        public bool IsSolo => Solo || SoloInverted;

        public bool IsBooted { get; set; } = true;

        public bool IsEnclosed => Filled || IsSolo || Outlined;

        public bool ShowLabel => HasLabel && !(IsSingle && LabelValue);

        public bool IsSingle => IsSolo || SingleLine || FullWidth || (Filled && !HasLabel);

        public virtual bool LabelValue => IsFocused || IsLabelActive || PersistentPlaceholder;

        public virtual ElementReference InputElement { get; set; }

        protected double LabelWidth { get; set; }

        public string LegendInnerHTML => "&#8203;";

        public bool HasCounter => Counter != false && Counter != null;

        public override string ComputedColor
        {
            get
            {
                if (!SoloInverted || !IsFocused)
                {
                    return base.ComputedColor;
                }

                return Color ?? "primary";
            }
        }

        public override bool HasColor => IsFocused;

        public virtual string Tag => "input";

        protected virtual Dictionary<string, object> InputAttrs => new(Attributes)
        {
            { "type", Type },
            { "value", _badInput == null ? InternalValue : _badInput }
        };

        public virtual StringNumber ComputedCounterValue
        {
            get
            {
                if (CounterValue != null)
                {
                    return CounterValue(InternalValue);
                }

                return InternalValue?.ToString()?.Length ?? 0;
            }
        }

        public override bool HasDetails => base.HasDetails || HasCounter;

        public StringNumber Max
        {
            get
            {
                int? max = null;

                if (Counter == true)
                {
                    if (Attributes.TryGetValue("maxlength", out var maxValue))
                    {
                        max = Convert.ToInt32(maxValue);
                    }
                }
                else
                {
                    max = Counter?.ToInt32();
                }

                return max == null ? null : (StringNumber)max;
            }
        }

        protected double PrefixWidth { get; set; }

        protected double PrependWidth { get; set; }

        protected (StringNumber left, StringNumber right) LabelPosition
        {
            get
            {
                var offset = (Prefix != null && !LabelValue) ? PrefixWidth : 0;

                if (LabelValue && PrependWidth > 0)
                {
                    offset -= PrependWidth;
                }

                return MasaBlazor.RTL == Reverse ? (offset, "auto") : ("auto", offset);
            }
        }

        public BLabel LabelReference { get; set; }

        public ElementReference PrefixElement { get; set; }

        public ElementReference PrependInnerElement { get; set; }

        protected virtual Dictionary<string, object> InputSlotAttrs { get; } = new();

        bool ITextField<TValue>.IsDirty => IsDirty;

        Dictionary<string, object> ITextField<TValue>.InputAttrs => InputAttrs;

        Dictionary<string, object> ITextField<TValue>.InputSlotAttrs => InputSlotAttrs;

        TValue ITextField<TValue>.InputValue => InputValue;

        protected override void SetComponentClass()
        {
            base.SetComponentClass();

            var prefix = "m-text-field";
            CssProvider
                .Merge(cssBuilder =>
                {
                    cssBuilder
                        .Add(prefix)
                        .AddIf($"{prefix}--full-width", () => FullWidth)
                        .AddIf($"{prefix}--prefix", () => Prefix != null)
                        .AddIf($"{prefix}--single-line", () => IsSingle)
                        .AddIf($"{prefix}--solo", () => IsSolo)
                        .AddIf($"{prefix}--solo-inverted", () => SoloInverted)
                        .AddIf($"{prefix}--solo-flat", () => Flat)
                        .AddIf($"{prefix}--filled", () => Filled)
                        .AddIf($"{prefix}--is-booted", () => IsBooted)
                        .AddIf($"{prefix}--enclosed", () => IsEnclosed)
                        .AddIf($"{prefix}--reverse", () => Reverse)
                        .AddIf($"{prefix}--outlined", () => Outlined)
                        .AddIf($"{prefix}--placeholder", () => Placeholder != null)
                        .AddIf($"{prefix}--rounded", () => Rounded)
                        .AddIf($"{prefix}--shaped", () => Shaped);
                })
                .Merge("text-field-details", cssBuilder =>
                {
                    cssBuilder
                        .Add("m-text-field__details");
                })
                .Merge("input-slot", cssBuilder =>
                {
                    cssBuilder
                        .AddBackgroundColor(BackgroundColor);
                })
                .Apply("prepend-inner", cssBuilder =>
                {
                    cssBuilder
                        .Add($"m-input__prepend-inner");
                })
                .Apply("append-inner", cssBuilder =>
                {
                    cssBuilder
                        .Add($"m-input__append-inner");
                })
                .Apply("append-outer-icon", cssBuilder =>
                {
                    cssBuilder
                        .Add("m-input__icon")
                        .Add("m-input__icon--append-outter");
                })
                .Apply("prepend-inner-icon", cssBuilder =>
                {
                    cssBuilder
                        .Add("m-input__icon")
                        .Add("m-input__icon--prepend-inner");
                })
                .Apply("clear-icon", cssBuilder =>
                {
                    cssBuilder
                        .Add("m-input__icon")
                        .Add("m-input__icon--clear");
                })
                .Apply("legend", styleAction: styleBuilder =>
                {
                    var width = (!SingleLine && (LabelValue || IsDirty)) ? LabelWidth : 0;
                    styleBuilder
                        .AddIf(() => $"width:{width}px", () => !IsSingle);
                })
                .Apply("legend-span", cssBuilder =>
                {
                    cssBuilder
                        .Add("notranslate");
                })
                .Apply("text-field-slot", cssBuilder =>
                {
                    cssBuilder
                        .Add("m-text-field__slot");
                })
                .Apply("text-field-prefix", cssBuilder =>
                {
                    cssBuilder
                        .Add("m-text-field__prefix");
                })
                .Apply("text-field-suffix", cssBuilder =>
                {
                    cssBuilder
                        .Add("m-text-field__suffix");
                });

            AbstractProvider
                .ApplyTextFieldDefault<TValue>()
                .ApplyTextFieldCounter(typeof(MCounter), attrs =>
                {
                    attrs[nameof(MCounter.Dark)] = Dark;
                    attrs[nameof(MCounter.Light)] = Light;
                    attrs[nameof(MCounter.Max)] = Max;
                    attrs[nameof(MCounter.Value)] = ComputedCounterValue;
                })
                .ApplyTextFieldLabel(typeof(MLabel), attrs =>
                {
                    var (left, right) = LabelPosition;

                    attrs[nameof(MLabel.Absolute)] = true;
                    attrs[nameof(MLabel.Focused)] = !IsSingle && (IsFocused || ValidationState != null);
                    attrs[nameof(MLabel.Left)] = left;
                    attrs[nameof(MLabel.Right)] = right;
                    attrs[nameof(MLabel.Value)] = LabelValue;
                })
                .ApplyTextFieldProcessLinear(typeof(MProgressLinear), attrs =>
                {
                    attrs[nameof(MProgressLinear.Absolute)] = true;
                    attrs[nameof(MProgressLinear.Color)] = (Loading == true || Loading == "") ? (Color ?? "primary") : Loading.ToString();
                    attrs[nameof(MProgressLinear.Height)] = LoaderHeight;
                    attrs[nameof(MProgressLinear.Indeterminate)] = true;
                })
                .ApplyTextFieldClearIcon(typeof(MIcon), attrs =>
                {
                    attrs[nameof(MIcon.Color)] = ValidationState;
                    attrs[nameof(MIcon.Dark)] = Dark;
                    attrs[nameof(MIcon.Disabled)] = Disabled;
                    attrs[nameof(MIcon.Light)] = Light;
                })
                .ApplyTextFieldAppendOuterIcon(typeof(MIcon), attrs =>
                {
                    attrs[nameof(MIcon.Color)] = ValidationState;
                    attrs[nameof(MIcon.Dark)] = Dark;
                    attrs[nameof(MIcon.Disabled)] = Disabled;
                    attrs[nameof(MIcon.Light)] = Light;
                })
                .ApplyTextFieldPrependInnerIcon(typeof(MIcon), attrs =>
                {
                    attrs[nameof(MIcon.Color)] = ValidationState;
                    attrs[nameof(MIcon.Dark)] = Dark;
                    attrs[nameof(MIcon.Disabled)] = Disabled;
                    attrs[nameof(MIcon.Light)] = Light;
                });
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                //OnAfterRender doesn't indicate DOM ready
                //So we should wait a little time
                //We may remove this when dialog been refactored
                await Task.Delay(16 * 3);

                var tasks = new Task[3];

                tasks[0] = SetLabelWidthAsync();
                tasks[1] = SetPrefixWidthAsync();
                tasks[2] = SetPrependWidthAsync();

                if (tasks.All(task => task.Status == TaskStatus.RanToCompletion || task.Status == TaskStatus.Canceled))
                {
                    return;
                }

                await Task.WhenAll(tasks);
                StateHasChanged();
            }
        }

        private async Task SetLabelWidthAsync()
        {
            if (!Outlined)
            {
                return;
            }

            //No label
            if (LabelReference == null || LabelReference.Ref.Id == null)
            {
                return;
            }

            var label = Document.GetElementByReference(LabelReference.Ref);
            var scrollWidth = await label.GetScrollWidthAsync();

            if (scrollWidth == null)
            {
                return;
            }

            var element = Document.GetElementByReference(Ref);
            var offsetWidth = await element.GetOffsetWidthAsync();

            if (offsetWidth == null)
            {
                return;
            }

            LabelWidth = Math.Min(scrollWidth.Value * 0.75 + 6, offsetWidth.Value - 24);
        }

        private async Task SetPrefixWidthAsync()
        {
            if (PrefixElement.Id == null)
            {
                return;
            }

            var prefix = Document.GetElementByReference(PrefixElement);
            var offsetWidth = await prefix.GetOffsetWidthAsync();

            if (offsetWidth == null)
            {
                return;
            }

            PrefixWidth = offsetWidth.Value;
        }

        private async Task SetPrependWidthAsync()
        {
            if (!Outlined)
            {
                return;
            }

            if (PrependInnerElement.Id == null)
            {
                return;
            }

            var prependInner = Document.GetElementByReference(PrependInnerElement);
            var offsetWidth = await prependInner.GetOffsetWidthAsync();

            if (offsetWidth == null)
            {
                return;
            }

            PrependWidth = offsetWidth.Value;
        }

        public virtual async Task HandleOnAppendOuterClickAsync(MouseEventArgs args)
        {
            if (OnAppendOuterClick.HasDelegate)
            {
                await OnAppendOuterClick.InvokeAsync(args);
            }
        }

        public virtual async Task HandleOnPrependInnerClickAsync(MouseEventArgs args)
        {
            if (OnPrependInnerClick.HasDelegate)
            {
                await OnPrependInnerClick.InvokeAsync(args);
            }
        }

        public override async Task HandleOnClickAsync(MouseEventArgs args)
        {
            if (IsFocused || IsDisabled)
            {
                return;
            }

            await InputElement.FocusAsync();
        }

        public virtual async Task HandleOnChangeAsync(ChangeEventArgs args)
        {
            if (OnChange.HasDelegate)
            {
                await OnChange.InvokeAsync(InternalValue);
            }
        }

        public virtual async Task HandleOnBlurAsync(FocusEventArgs args)
        {
            _badInput = null;
            IsFocused = false;

            if (OnBlur.HasDelegate)
            {
                await OnBlur.InvokeAsync(args);
            }
        }

        public virtual async Task HandleOnInputAsync(ChangeEventArgs args)
        {
            //Since event will call StateHasChanged,we should prevent it
            //So that,view will not change untill 300ms no actions
            _shouldRender = false;

            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            await Task.Delay(300, _cancellationTokenSource.Token);

            var success = BindConverter.TryConvertTo<TValue>(args.Value, System.Globalization.CultureInfo.InvariantCulture, out var val);
            if (success)
            {
                _badInput = null;
                await SetInternalValueAsync(val);

                if (OnInput.HasDelegate)
                {
                    await OnInput.InvokeAsync(InternalValue);
                }
            }
            else
            {
                _badInput = args.Value.ToString();
            }

            _shouldRender = true;
            StateHasChanged();
        }

        protected override bool ShouldRender()
        {
            return _shouldRender;
        }

        public virtual async Task HandleOnFocusAsync(FocusEventArgs args)
        {
            if (!IsFocused)
            {
                IsFocused = true;
                if (OnFocus.HasDelegate)
                {
                    await OnFocus.InvokeAsync(args);
                }
            }
        }

        public virtual async Task HandleOnKeyDownAsync(KeyboardEventArgs args)
        {

            if (OnKeyDown.HasDelegate)
            {
                await OnKeyDown.InvokeAsync(args);
            }
        }

        public virtual async Task HandleOnClearClickAsync(MouseEventArgs args)
        {
            await SetInternalValueAsync(default);

            if (OnClearClick.HasDelegate)
            {
                await OnClearClick.InvokeAsync(args);
            }
        }
    }
}
