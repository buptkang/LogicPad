   M S B A M L   `   `   `  ���� M  IDiagramUserControl, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:LogicPad2.DiagramLogicPad2.Diagram  $    LogicPad2.Diagram.UserControl1   R NWindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    8x,http://schemas.microsoft.com/winfx/2006/xaml    Hmc;http://schemas.openxmlformats.org/markup-compatibility/2006    >d2http://schemas.microsoft.com/expression/blend/2008    /localclr-namespace:LogicPad2.Diagram      � 	Resources     TButtonStyle �� 
TargetType �� BasedOn �� Property �� Value �� Triggers �� Binding �� RelativeSource �� Path	 �� Value��%�   &	        7 ��� " ��8   .���  �@^�ShowOnDisabled .  ��  ��  �� *Self+$ 	IsEnabled�		 False.a���  �@J�Opacity$
 0.50Q����� 
 �� ImageSource$
 Images/metal.jpg�� j� Stretch$
 None=� j� TileMode$
 Tile=� j� Viewport$ 1,1,211,211� j� ViewportUnits$ Absolute=�z�V� .~��� -   UserControlScaleXY�� �� ScaleX$ 1Q� �� ScaleY$ 1Q�T� -   UserControlXY�� T� X$ 0Q� T� Y$ 0Q�.���� -    ��Name$ controlBorder��$��1q�
���   �._�� -    �Name$ Grid0�� � RowDefinitions �� $
��Auto �$	��450���� $��28 � � ColumnDefinitions �� $	��850��.+��� $��1q�
���   �$��0��._��� $
��Auto=�$
��Auto=�.��� -   $��Grid1�� �� $
��Auto ��� $��* � �� $
��Auto ��� $��* �.+�%   LogicPad2.Diagram.GateInkCanvas  -     Name$ 	inkCanvas��$��1��$��1��$��2��+   %LogicPad2.Diagram.PieMenuGateSelector  -     Name$ pieMenuGateSelector��$��1��$��1��$y�	Collapsed=�$��2���� -    ��Name$ adornerLayer��$y�	Collapsed=��� $��1��$��1q�
���   �._��� -	   $��controlRegion�� �� Orientation$ 
Horizontal=�$��Right=�$	��Top=�.+��� $��Logic Diagram��$
��Bold"�$��Italic$� J� Opacity$	 0.7Q��� -
    ��Name$ btnSave��$��
170,0,10,0q� � ToolTip$ Save As Image��$	 0.5Q�.���� $��Images/saveAsHS.png���� -   $ 	btnCancel��$��0,0,10,0q�$ Close InkCanvas��$	 0.5Q�.���� $��Images/cancel.png���� -   $ btnTransform��$��0,0,0,0q�$ Move InkCanvas��$	 0.5Q�.���� $��Images/hand_tool.png���� -   $ btnScale��$��10,0,0,0q�$ Scale InkCanvas��$	 0.5Q�.���� $��Images/Scale.png��