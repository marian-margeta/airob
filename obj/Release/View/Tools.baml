   M S B A M L   `   `   `  ���� @  <Airob, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:Airob.View
Airob.View  3clr-namespace:Airob.ConvertersAirob.Converters  /clr-namespace:Airob.ServicesAirob.Services      Airob.View.Tools   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     c _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  f bGalaSoft.MvvmLight.Platform, Version=5.3.0.19032, Culture=neutral, PublicKeyToken=5f873c45e98af8a1(c http://www.galasoft.ch/mvvmlight  $localclr-namespace:Airob.View  /
convertersclr-namespace:Airob.Converters  +servicesclr-namespace:Airob.Services    � DataContext  �� *Main+   Locator �� Source# [   � 	Resources  �� 
TargetType �� BasedOn �� Property �� Value*  $Airob.Converters.EnumToBoolConverter  EnumToBoolConverter��%�   '	�       7d�&	 l     �� " 	�8   .���  � �� Z� ��  � ��
 ���� ��  � ��$ 0q���  � ��$	 4,5q�  .��� .+��� 
�������$��2q� �� CornerRadius$ 5�����	#FFEAEAEA$	��Top=�$��3q�._��� $	��Top=� �� Orientation$ Vertical=�$��0q�.+�	� 	 � ToolTip$	 Move objects��
 d� 	IsChecked
 ��  �� Path$ AppState.EditState� �� 	Converter# [    Airob.Services.EditState   Motion �� ConverterParameter# Z .��F� $��18�� �� Stretch$ Uniform=�
���   �����p���z6��P@���� c������ߧ� �J�ߧ� �J��Y�4S��Y�z6��P@�j�  �>� c���Y�4ߧ��Y�4ߧ�ߧ���Y�ߧ���Y c�Q    �z6��P@���Y�S���Y �J�4ߧ �J�4ߧ���� c������z6��P@���4S���� �J���� �J �J���� �J�����SQ���z6��P@	� $	 Rotation of the robot��
 �� $ AppState.EditState�# [    Rotation# Z .��F� $��16��$ Uniform=�
���   ������ �3 *Ә�2h�;P '�AX�N\)o�Ȝ� �\� ������  Ւ�j>� ����{ �X�d \)oӬ�M �AX��A �;� A  *���A 0#� t�M �� �Md � Ӥ�z e� ���  p� �� �7� �l�� ����Ic��~ Q��� �j�   ?�(2� 7_ ��Q� R_ ȳ� �c Xr 5m � �[ >nv ��G ��� ,6 :�� Ӕ� ̵� ��t�7�?���  *��8C��?x?�l2�� XaJ� �a�Tb 8�w&�$ �,6 ����v�G tg�J'\ 4ۼ��r L��pt� d/�i� L����� ����L�� L����� d/���L��`04ۼ�f-tg��,?���Ӹ�P���]8�w �f�a�L�oXaJ �t�l2��t *� �3 *	� $	 Add new points to line��
 �� $ AppState.EditState�# [    AddPoint# Z .��� .+�F� $��16��$ Uniform=� � UseLayoutRounding .  
���   �������}�(?��    (?��    ɬѰ���� ь@�`����}�(?���d(�(�$8��{�0�!2� ��|t\W8 ��|t\W8 �٨�
���QI��NP@    ��\W8 ��\W8 Ѡ�d�O�Ӡ�d�O���Xڷ����Ӝ�S���d(�(��d(�(	� $	 Remove line��.  (System.Windows.Interactivity.Interaction   Triggers /  )System.Windows.Interactivity.EventTrigger     	EventName$ Checked��.  (System.Windows.Interactivity.TriggerBase   Actions. /  )GalaSoft.MvvmLight.Command.EventToCommand     Command �� *RemoveCommand+.��F� $��15��$ Uniform=� .  
���   ������@�T�[ Ә(6T�[  �JZp  �JX� � �J��� �(6�� @��� ���- �� �2} �� ��� X� �    Zp 2} T�[ ��- T�[ р�T�[ р���- Ӏ�ڄ �%'A_p�  P?@o@A_p�  P?Q�2
A_p�  P?S�#A_p�  P?��7ڄ ��7��- р�7T�[ �@�T�[ �P}��JQ��7��A϶P@�H�sh�7ј�7��s�P}��T%� Q% ���� a�T%ј���s����h�7р��)��� a�J�M%�u��P}��J�[ hN]Ѭ�[ h��-���P@h��-���P@hN]Ѭ�[ hN]	� $��30��$��30��$	 Add barrier��
 �� $ AppState.EditState�# [    
AddBarrier# Z .���� $	��910��$	��910��z�V� .~���  �� ScaleX$ 0.018Q� �� ScaleY$ 0.018Q�T�  T� X$ 3Q�.+�F�  .  
���   �������In
@��,�@��	@��,�@��	�\�@��\�@�@��,��M�@��,� u@��,� ��2k-� �@.�� ����.�� ����/ u��0�M���0��In
��0Ӏ"��0�v����/�v����.��v�@.��v��2k-�"@��,�In
@��,	F�  .  
���   ��������2@��,� b2@��,� b2�\� �-�\� �-@��,р:,@��,��,�+@��,��+�2k-��+@.р�+���.Ӏ�+���/�,�+��0�:,��0р�2��0�@ڎ3��0�.4���/�.4���.р.4@.Ӏ.4�2k-@ڎ3@��,��2@��,	�  .  
���   � � PointsO �   @
G��# �C��#����#��� �& �� <� �� <�@
G �&�  .  
���   �; �   `�S#��#�e���#���! <� ^�) <�`�S# �&F�  .  
���   �f������8��#���5��#� � ��#�Z��3M�    ���    @�S��J @u��y�  <��୎ <�р�8��#	F�  .  
���   ������ m5��#��=�*��#� �1 <��������@ <��@i�5@u������o�@@�S�����o�@��������o�@�3MӀ=6�Z��E�5��# m5��#	�� $��0q�	� $	 Run simulation��$��30��
 �� $ AppState.EditState�# [    
Simulation# Z .��� $��13��$ Uniform=� .  
���   � �   ��� �pr ��� 