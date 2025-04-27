using Bunit;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using SharedLibrary.Components;
using Xunit.Abstractions;
using Moq;

namespace SharedLibraryTests.Component;

public class ButtonTests : TestContext
{
   private readonly TestBaseHelper _output;

    public ButtonTests(ITestOutputHelper output)
    {
        _output = new TestBaseHelper(output);
    }

    protected IRenderedComponent<Button> RenderButton(
        RenderFragment? childContent = null,
        Func<MouseEventArgs, Task>? onClick = null)
    {
        return RenderComponent<Button> (parameters => parameters
            .Add(p => p.ChildContent, childContent)
            .Add(p => p.OnClick, onClick));
    }

    [Fact]
    public void TestCreateButton()
    {
        // Arrange
        var mockClick = new Mock<Func<MouseEventArgs, Task>>();
        var content = "<p>★</p>";
        var cut = RenderButton(
           childContent: builder => builder.AddMarkupContent(0, content),
           onClick: mockClick.Object);

        // Act
        cut.Find("button").Click(new MouseEventArgs());

        // Assert

    }
}
