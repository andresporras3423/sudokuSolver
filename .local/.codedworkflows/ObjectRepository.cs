using UiPath.CodedWorkflows.DescriptorIntegration;

namespace sudokuSolver.ObjectRepository
{
    public static class Descriptors
    {
        public static class __Chrome_YouTube
        {
            static string _reference = "iF6Y1CLUi0-BnmuVdif5mw/IBS20_vHE0q4MQgwVyEDZA";
            public static _Implementation.___Chrome_YouTube.__Chrome_YouTube Chrome_YouTube { get; private set; } = new _Implementation.___Chrome_YouTube.__Chrome_YouTube();
        }

        public static class __Sudoku
        {
            static string _reference = "iF6Y1CLUi0-BnmuVdif5mw/pb6_0LzSHEuSRZqNWsD2fQ";
            public static _Implementation.___Sudoku.__Sudoku Sudoku { get; private set; } = new _Implementation.___Sudoku.__Sudoku();
        }
    }
}

namespace sudokuSolver._Implementation
{
    internal class ScreenDescriptorDefinition : IScreenDescriptorDefinition
    {
        public IScreenDescriptor Screen { get; set; }

        public string Reference { get; set; }

        public string DisplayName { get; set; }
    }

    internal class ElementDescriptorDefinition : IElementDescriptorDefinition
    {
        public IScreenDescriptor Screen { get; set; }

        public string Reference { get; set; }

        public string DisplayName { get; set; }

        public IElementDescriptor ParentElement { get; set; }

        public IElementDescriptor Element { get; set; }
    }

    namespace ___Chrome_YouTube._Chrome_YouTube
    {
        public class __Label : IElementDescriptor
        {
            private readonly IScreenDescriptor _screenDescriptor;
            private readonly IElementDescriptor _parentElementDescriptor;
            private readonly IElementDescriptorDefinition _elementDescriptor;
            public IElementDescriptorDefinition GetDefinition()
            {
                return _elementDescriptor;
            }

            public __Label(IScreenDescriptor screenDescriptor, IElementDescriptor parentElementDescriptor)
            {
                _screenDescriptor = screenDescriptor;
                _parentElementDescriptor = parentElementDescriptor;
                _elementDescriptor = new ElementDescriptorDefinition{Reference = "iF6Y1CLUi0-BnmuVdif5mw/XnhMyLa9U0-3U-ioxMtdsw", DisplayName = "Label", Element = this, ParentElement = _parentElementDescriptor, Screen = screenDescriptor};
            }
        }
    }

    namespace ___Chrome_YouTube
    {
        public class __Chrome_YouTube : IScreenDescriptor
        {
            public IScreenDescriptorDefinition GetDefinition()
            {
                return _screenDescriptor;
            }

            private readonly ScreenDescriptorDefinition _screenDescriptor;
            public __Chrome_YouTube()
            {
                _screenDescriptor = new ScreenDescriptorDefinition{Reference = "iF6Y1CLUi0-BnmuVdif5mw/wtNjWnK1OE6dUASBFgvQBA", DisplayName = "Chrome YouTube", Screen = this};
                Label = new _Implementation.___Chrome_YouTube._Chrome_YouTube.__Label(this, null);
            }

            public _Implementation.___Chrome_YouTube._Chrome_YouTube.__Label Label { get; private set; }
        }
    }

    namespace ___Sudoku._Sudoku
    {
        public class __DIV : IElementDescriptor
        {
            private readonly IScreenDescriptor _screenDescriptor;
            private readonly IElementDescriptor _parentElementDescriptor;
            private readonly IElementDescriptorDefinition _elementDescriptor;
            public IElementDescriptorDefinition GetDefinition()
            {
                return _elementDescriptor;
            }

            public __DIV(IScreenDescriptor screenDescriptor, IElementDescriptor parentElementDescriptor)
            {
                _screenDescriptor = screenDescriptor;
                _parentElementDescriptor = parentElementDescriptor;
                _elementDescriptor = new ElementDescriptorDefinition{Reference = "iF6Y1CLUi0-BnmuVdif5mw/kPvG9hrgaUmaG4CwPPBbug", DisplayName = "DIV", Element = this, ParentElement = _parentElementDescriptor, Screen = screenDescriptor};
            }
        }
    }

    namespace ___Sudoku
    {
        public class __Sudoku : IScreenDescriptor
        {
            public IScreenDescriptorDefinition GetDefinition()
            {
                return _screenDescriptor;
            }

            private readonly ScreenDescriptorDefinition _screenDescriptor;
            public __Sudoku()
            {
                _screenDescriptor = new ScreenDescriptorDefinition{Reference = "iF6Y1CLUi0-BnmuVdif5mw/3MxGEas-bEa3fwQu7iNkxw", DisplayName = "Sudoku", Screen = this};
                DIV = new _Implementation.___Sudoku._Sudoku.__DIV(this, null);
            }

            public _Implementation.___Sudoku._Sudoku.__DIV DIV { get; private set; }
        }
    }
}