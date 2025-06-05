using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using Moq;
using Bunit;
using Xunit.Abstractions;
using AWUI.JsInterop;
using AWUI.Options;
using AWUI.Components;
using AWUI.Interfaces;

namespace SharedLibraryTests.Component;

public class ButtonTests : TestContext
{
    private readonly TestBaseHelper _output;
    private readonly Mock<IEventBus> _eventBusMock;
    private readonly Mock<IJSRuntime> _jsRuntimeMock;
    private readonly AWJsInterop _jsInterop;
    private readonly IStringLocalizer<SecureComponentBase> _localizer;

    public ButtonTests(ITestOutputHelper output)
    {
        _output = new TestBaseHelper(output);

        _eventBusMock = new Mock<IEventBus>();
        _jsRuntimeMock = new Mock<IJSRuntime>();

        // AWJsInterop
        var options = Options.Create(new JsModuleOptions
        {

        });
        _jsInterop = new AWJsInterop(_jsRuntimeMock.Object, options);

        // Localizer
        var localizedString = new LocalizedString("TestKey", "TestValue");
        var localizerMock = new Mock<IStringLocalizer<SecureComponentBase>>();
        localizerMock.Setup(l => l[It.IsAny<string>()])
            .Returns((string key) => localizedString);
        _localizer = localizerMock.Object;

        Services.AddSingleton(_eventBusMock.Object);
        Services.AddSingleton<IJSInterop>(_jsInterop);
        Services.AddSingleton(_jsInterop);
        Services.AddSingleton(_localizer);

        _jsRuntimeMock.Setup(x => x.InvokeAsync<IJSObjectReference>(
                "import", It.IsAny<object[]>()))
            .ReturnsAsync(Mock.Of<IJSObjectReference>());
    }

    protected IRenderedComponent<Button> RenderButton(
        EventCallback onClick,
        RenderFragment? childContent = null)
    {
        return RenderComponent<Button>(parameters =>
        {
            if (childContent != null)
                parameters.Add(p => p.ChildContent, childContent);

            if (onClick.HasDelegate)
                parameters.Add(p => p.OnClick, onClick);
        });
    }

    [Fact]
    public async Task TestCreateButton()
    {
        // Arrange
        var mockHandler = new Mock<IHandleEvent>();
        var content = "<p>★</p>";

        mockHandler.Setup(x => x.HandleEventAsync(
            It.IsAny<EventCallbackWorkItem>(),
            It.IsAny<object>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        var cut = RenderButton(
            childContent: builder => builder.AddMarkupContent(0, content),
            onClick: EventCallback.Factory.Create(mockHandler.Object, () => { }));

        // Act
        await cut.Find("button").ClickAsync(new MouseEventArgs());

        // Assert
        mockHandler.Verify(x => x.HandleEventAsync(It.IsAny<EventCallbackWorkItem>(), It.IsAny<object>()), Times.Once);
    }
}
