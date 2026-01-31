using Godot;

[GlobalClass]
public partial class EventAudioData : Resource
{
	[Export] public MaskType EventType { get; set; } = MaskType.None;
	[Export] public AudioStream LoopingSound { get; set; }
	[Export] public AudioStream OneShotSound { get; set; }
}
