   M S B A M L   `   `   `  ����   GatesWpf    GatesWpf.SaveClose   \ XPresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    8x,http://schemas.microsoft.com/winfx/2006/xaml      � Title$  Save Changes?��$	��150��$	��400�� � 
ResizeMode$ NoResize=� � Icon$ Images/ram.png��.���  � RowDefinitions �� $
��Auto ��� $
��Auto ��� $
��Auto � � ColumnDefinitions �� $
��Auto ��� $��* �.+��� $��Images/Hard_drive.png��$��2��$��15,15,15,15q��� -   $��
lblCircuit��$��1��$
��Wrap=�$��Arial)�$
��Bold"�$��	0,15,15,0q�><Do you want to save the changes you made in the circuit {0}?�� $��1��$��1��$
��Wrap=�$��Arial)�$��	0,10,15,0q�31Your changes will be lost if you don't save them.� $��1��$��2�� �� $��75 ��� $��* ��� $��75 ��� $��* ��� $��50 ��� $��* �.+��� -   $��btnSave��$��0�� �� 	IsDefault . .��_Save�� -   $��btnDontSave��$��2��.��_Don't Save�� -   $��	btnCancel��$��4�� �� IsCancel . .��	_Cancel