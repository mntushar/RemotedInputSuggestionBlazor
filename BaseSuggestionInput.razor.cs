using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;

namespace DNE.CS.Inventory.Client.Pages.Shared.Component;

public delegate ValueTask<ItemsProviderResult<InputSelectElement>> BaseSuggestionInputItemsProvider(
    InputRequest request);

public partial class BaseSuggestionInput
{
    private bool _isSelectItemBoxActive = true;
    private InputSelectElement _context = new InputSelectElement();
    private Virtualize<InputSelectElement>? VirtualRef;
    [Parameter]
    public string? SelectId { get; set; }
    [Parameter]
    public string? Placeholder { get; set; }
    [Parameter]
    public string? SelectedItem { get; set; }
    [Parameter]
    public Delegate? InputProvider { get; set; }
    [Parameter]
    public EventCallback<InputSelectElement> SelectedOutput { get; set; }


    private void Search()
    {
        if (VirtualRef != null)
            _ = VirtualRef.RefreshDataAsync();
    }

    private void SelectInputFocuse()
    {
        _isSelectItemBoxActive = true;
    }

    private async Task SelectData(InputSelectElement element)
    {
        _isSelectItemBoxActive = false;
        await SelectedOutput.InvokeAsync(element);
    }

    public async Task Refresh()
    {
        if (VirtualRef != null)
        {
            await VirtualRef.RefreshDataAsync();
            StateHasChanged();
        }
    }

    private async ValueTask<ItemsProviderResult<InputSelectElement>> ProvideVirtualizedItemsAync(
        ItemsProviderRequest request)
    {
        if (request.CancellationToken.IsCancellationRequested) return default;

        if (InputProvider != null)
        {
            InputRequest inputRequest = new InputRequest()
            {
                StartIndex = request.StartIndex,
                Count = request.Count,
                CancellationToken = request.CancellationToken,
                SearchKey = SelectedItem
            };
            object?[]? objects = { inputRequest };
            object? result = InputProvider.DynamicInvoke(objects);

            if (result is ValueTask<ItemsProviderResult<InputSelectElement>> valueTaskResult)
            {
                return await valueTaskResult;
            }
            else
            {
                throw new Exception("SelectItemProvider is not a SelectElement delegate type");
            }
        }

        return new ItemsProviderResult<InputSelectElement>(new List<InputSelectElement>(), 0);
    }
}

public class InputSelectElement
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class InputRequest
{
    public int StartIndex { get; init; }
    public int Count { get; init; }
    public CancellationToken CancellationToken { get; init; }
    public string? SearchKey { get; init; }
}

