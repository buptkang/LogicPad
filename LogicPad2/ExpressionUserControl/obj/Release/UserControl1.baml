   M S B A M L   `   `   `  ���� E  AInq, Version=1.0.4287.38769, Culture=neutral, PublicKeyToken=null<)clr-namespace:starPadSDK.Inq;assembly=InqstarPadSDK.Inq  B >WPFHelp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullH1clr-namespace:starPadSDK.WPFHelp;assembly=WPFHelpstarPadSDK.WPFHelp  ExpressionUserControl'   !LogicPad2.Expression.UserControl1   R NWindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    8x,http://schemas.microsoft.com/winfx/2006/xaml    Hmc;http://schemas.openxmlformats.org/markup-compatibility/2006    >d2http://schemas.microsoft.com/expression/blend/2008    BStarPadSDK_Inq)clr-namespace:starPadSDK.Inq;assembly=Inq    CWPFHelp1clr-namespace:starPadSDK.WPFHelp;assembly=WPFHelp    -     >�Name$  expressionUserControl������  �� ImageSource$ Images/metal.jpg�� j� Stretch$
 None=� j� TileMode$
 Tile=� j� Viewport$ 1,1,211,211� j� ViewportUnits$ Absolute=�z�V� .~��� -   UserControlScaleXY�� �� ScaleX$ 1Q� �� ScaleY$ 1Q�T� -   UserControlXY�� T� X$ 0Q�	 T� Y$	 0Q�.���� -   
 ��Name$
 controlBorder��$��1q�
���   �._�� -    �Name$ Grid0�� � RowDefinitions �� $
��Auto ��� $��28 �.+�]� $��0��.+�`� -    `�Name$ alternatesTray��$��Right=� `� Orientation$ Vertical=�.�c� -    c�Name$ alternatesMenu��$��Times New Roman)��� -   $��
GCScroller��$
��Auto=�$
��Auto=�.��� }�. .+�,  &starPadSDK.WPFHelp.ContainerVisualHost  -	     Name$ underlay��$��Stretch=�$
��Auto�� J� OpacityMask Z�    starPadSDK.Inq.InqCanvas  -
     Name$ 	inqCanvas�����	#00000000$	��200���� $��1��$��1q�
���   �._��� -   $��controlRegion�� �� Orientation$ 
Horizontal=�$��Right=�$	��Top=�.+��� $��Logic Expression��$
��Bold"�$��Italic$� J� Opacity$	 0.7Q��� -    ��Name$ btnSave��$��
170,0,10,0q� � ToolTip$ Save As Image��$	 0.5Q�.���� $��Images/saveAsHS.png���� -   $ 	btnCancel��$��0,0,10,0q�$ Close InkCanvas��$	 0.5Q�.���� $��Images/cancel.png���� -   $ btnTransform��$��0,0,0,0q�$ Move InkCanvas��$	 0.5Q�.���� $��Images/hand_tool.png���� -   $ btnScale��$��10,0,0,0q�$ Scale InkCanvas��$	 0.5Q�.���� $��Images/Scale.png��