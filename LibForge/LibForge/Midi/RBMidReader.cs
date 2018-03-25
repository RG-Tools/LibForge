﻿using System;
using System.Collections.Generic;
using System.IO;
using LibForge.Util;
using MidiCS;
using MidiCS.Events;

namespace LibForge.Midi
{
  public class RBMidReader : ReaderBase
  {
    public static RBMid ReadStream(Stream s)
    {
      return new RBMidReader(s).Read();
    }
    public RBMidReader(Stream s) : base(s) { }

    const uint MaxInstTracks = 10;
    const uint MaxKeysAnimTracks = 2;
    const uint MaxVocalTracks = 4;
    private RBMid Read()
    {
      var r = new RBMid();
      r.Format = Check(Int(), 16);
      r.Lyrics = Arr(ReadLyrics, MaxInstTracks);
      r.DrumFills = Arr(ReadDrumFills, MaxInstTracks);
      r.Anims = Arr(ReadAnims, MaxKeysAnimTracks);
      r.ProMarkers = Arr(ReadMarkers, MaxInstTracks);
      r.LaneMarkers = Arr(ReadUnktrack, MaxInstTracks);
      r.TrillMarkers = Arr(ReadUnktrack2, MaxInstTracks);
      r.DrumMixes = Arr(ReadDrumMixes, MaxInstTracks);
      r.GemTracks = Arr(ReadGemTracks, MaxInstTracks);
      r.OverdriveSoloSections = Arr(ReadOverdrives, MaxInstTracks);
      r.VocalTracks = Arr(ReadVocalTrack, MaxVocalTracks);
      r.Unknown1 = Int();
      r.Unknown2 = Int();
      r.Unknown3 = Float();
      r.Unknown4 = Int();
      r.Unknown5 = Int();
      r.Unknown6 = Int();
      r.Unknown7 = Int();
      r.Unknown8 = Float();
      r.Unknown9 = Float();
      r.Unknown10 = Int();
      r.NumPlayableTracks = Int();
      r.Unknown12 = Int();
      r.Unknown13 = Byte();
      r.PreviewStartMillis = Float();
      r.PreviewEndMillis = Float();
      r.GuitarHandmap = Arr(ReadHandMap, MaxInstTracks);
      r.GuitarLeftHandPos = Arr(ReadHandPos, MaxInstTracks);
      r.Unktrack = Arr(ReadUnkTrack, MaxInstTracks);

      r.MarkupSoloNotes1 = Arr(ReadMarkupSoloNotes);
      r.TwoTicks1 = Arr(ReadTwoTicks);
      r.MarkupChords1 = Arr(ReadMarkupChord);
      r.MarkupSoloNotes2 = Arr(ReadMarkupSoloNotes);
      r.MarkupSoloNotes3 = Arr(ReadMarkupSoloNotes);
      r.TwoTicks2 = Arr(ReadTwoTicks);

      r.UnknownTwo = Check(Int(), 2);
      r.Unknown16 = Int();
      r.MidiTracks = Arr(ReadMidiTrack);
      r.UnknownInts = FixedArr(Int, 9);
      r.UnknownFloats = FixedArr(Float, 4);
      r.Tempos = Arr(ReadTempo);
      r.TimeSigs = Arr(ReadTimesig);
      r.Beats = Arr(ReadBeat);
      r.Unknown19 = Check(Int(), 0);
      r.MidiTrackNames = CheckedArr(String, (uint)r.MidiTracks.Length);
      return r;
    }
    private RBMid.TICKTEXT ReadTickText() => new RBMid.TICKTEXT
    {
      Tick = UInt(),
      Text = String()
    };
    private RBMid.LYRICS ReadLyrics() => new RBMid.LYRICS
    {
      TrackName = String(),
      Lyrics = Arr(ReadTickText),
      Unknown1 = Int(),
      Unknown2 = Int(),
      Unknown3 = Byte()
    };
    private RBMid.DRUMFILLS ReadDrumFills() => new RBMid.DRUMFILLS
    {
      Unknown = Arr(() => new RBMid.DRUMFILLS.FILLS_UNK
      {
        Tick = UInt(),
        Unknown = UInt()
      }),
      Fills = Arr(() => new RBMid.DRUMFILLS.DRUMFILL
      {
        StartTick = UInt(),
        EndTick = UInt(),
        IsBRE = Byte()
      })
    };
    private RBMid.ANIM ReadAnims() => new RBMid.ANIM
    {
      TrackName = String(),
      Unknown1 = Int(),
      Unknown2 = Int(),
      Events = Arr(() => new RBMid.ANIM.EVENT
      {
        Time = Float(),
        Tick = UInt(),
        Unknown1 = Int(),
        KeyBitfield = Int(),
        Unknown2 = Int(),
        Unknown3 = Short()
      }),
      Unknown3 = Int()
    };
    private RBMid.CYMBALMARKER ReadMarkers() => new RBMid.CYMBALMARKER
    {
      Markers = Arr(() => new RBMid.CYMBALMARKER.MARKER
      {
        Tick = UInt(),
        Flags = (RBMid.CYMBALMARKER.MARKER.FLAGS)Int()
      }),
      Unknown1 = Int(),
      Unknown2 = Int()
    };
    private RBMid.LANEMARKER ReadUnktrack() => new RBMid.LANEMARKER
    {
      Markers = Arr(() => Arr(() => new RBMid.LANEMARKER.MARKER
      {
        StartTick = UInt(),
        EndTick = UInt(),
        Flags = (RBMid.LANEMARKER.MARKER.Flag)UInt()
      }))
    };
    private RBMid.GTRTRILLS ReadUnktrack2() => new RBMid.GTRTRILLS
    {
      Trills = Arr(() => Arr(() => new RBMid.GTRTRILLS.TRILL
      {
        StartTick = UInt(),
        EndTick = UInt(),
        LowFret = Int(),
        HighFret = Int()
      }))
    };
    private RBMid.DRUMMIXES ReadDrumMixes() => new RBMid.DRUMMIXES
    {
      Mixes = Arr(() => Arr(ReadTickText))
    };
    private RBMid.GEMTRACK ReadGemTracks() => new RBMid.GEMTRACK
    {
      Gems = Arr(Seq(Skip(4), () => Arr(() => new RBMid.GEMTRACK.GEM
      {
        StartMillis = Float(),
        StartTicks = UInt(),
        Unknown = Short(),
        Unknown2 = Short(),
        Unknown3 = Byte(),
        Unknown4 = Int(),
        Unknown5 = Byte(),
        Unknown6 = Int()
      }))),
      Unknown = Int()
    };
    private RBMid.SECTIONS ReadOverdrives() => new RBMid.SECTIONS
    {
      Sections = Arr(() => Arr(() => Arr(() => new RBMid.SECTIONS.SECTION
      {
        StartTicks = UInt(),
        LengthTicks = UInt()
      })))
    };
    private RBMid.VOCALTRACK ReadVocalTrack() => new RBMid.VOCALTRACK
    {
      PhraseMarkers = Arr(ReadPhraseMarker),
      PhraseMarkers2 = Arr(ReadPhraseMarker),
      Notes = Arr(() => new RBMid.VOCALTRACK.VOCAL_NOTE
      {
        Type = Int(),
        MidiNote = Int(),
        MidiNote2 = Int(),
        StartMillis = Float(),
        StartTick = UInt(),
        Length = Float(),
        Unknown1 = Short(),
        Lyric = String(),
        Unknown2 = FixedArr(Byte, 9)
      }),
      Unknown1 = Arr(Int),
      Unknown2 = Arr(() => new RBMid.VOCALTRACK.UNKNOWN
      {
        Unknown1 = Float(),
        Unknown2 = Float()
      })
    };
    private RBMid.VOCALTRACK.PHRASE_MARKER ReadPhraseMarker() =>
      new RBMid.VOCALTRACK.PHRASE_MARKER
      {
        StartMillis = Float(),
        Length = Float(),
        Unknown1 = Int(),
        Unknown2 = Int(),
        Unknown3 = Int(),
        Unknown4 = Int(),
        Unknown5 = Int(),
        Unknown6 = FixedArr(Byte, 25)
      };
    private RBMid.HANDMAP ReadHandMap() => new RBMid.HANDMAP
    {
      Maps = Arr(() => new RBMid.HANDMAP.MAP
      {
        StartMillis = Float(),
        Map = Int()
      })
    };
    private RBMid.HANDPOS ReadHandPos() => new RBMid.HANDPOS
    {
      Events = Arr(() => new RBMid.HANDPOS.POS
      {
        StartMillis = Float(),
        EndMillis = Float(),
        Position = Int(),
        Unknown = Byte()
      })
    };
    private RBMid.UNKTRACK ReadUnkTrack() => new RBMid.UNKTRACK
    {
      Unknown = Arr(Float)
    };
    private RBMid.MARKUP_SOLO_NOTES ReadMarkupSoloNotes() => new RBMid.MARKUP_SOLO_NOTES
    {
      StartTick = UInt(),
      EndTick = UInt(),
      NoteOffset = Int()
    };
    private RBMid.TWOTICKS ReadTwoTicks() => new RBMid.TWOTICKS
    {
      Tick1 = UInt(),
      Tick2 = UInt()
    };
    private RBMid.MARKUPCHORD ReadMarkupChord() => new RBMid.MARKUPCHORD
    {
      StartTick = UInt(),
      EndTick = UInt(),
      Pitches = Arr(Int)
    };
    private uint midiTick;
    private string trackName;
    private string[] trackStrings;
    private MidiTrack ReadMidiTrack()
    {
      var unk = Byte();
      var unk2 = Int();
      var num_events = UInt();
      midiTick = 0;
      trackName = "";
      var start = s.Position;
      s.Position += num_events * 8;
      trackStrings = Arr(String);
      var end = s.Position;
      s.Position = start;
      var msgs = new List<IMidiMessage>(FixedArr(ReadMessage, num_events));
      msgs.Add(new EndOfTrackEvent(0));
      s.Position = end;
      return new MidiTrack(msgs, midiTick, trackName);
    }
    private IMidiMessage ReadMessage()
    {
      var tick = UInt();
      var deltaTime = tick - midiTick;
      midiTick = tick;
      var kind = Byte();
      switch (kind)
      {
        // Midi Messages
        case 1:
          var tc = Byte();
          var channel = (byte)(tc & 0xF);
          var type = tc >> 4;
          var note = Byte();
          var velocity = Byte();
          switch (type)
          {
            case 8:
              return new NoteOffEvent(deltaTime, channel, note, velocity);
            case 9:
              return new NoteOnEvent(deltaTime, channel, note, velocity);
            case 12: // seen in foreplaylongtime, assuming prgmchg
              return new ProgramChgEvent(deltaTime, channel, note);
            default:
              throw new NotImplementedException($"Message type {type}");
          }
        // Tempo
        case 2:
          var tempo_msb = (uint)Byte();
          var tempo_lsb = UShort();
          return new TempoEvent(deltaTime, tempo_msb << 16 | tempo_lsb);
        // Time Signature
        case 4:
          var num = Byte();
          var denom = Byte();
          var denom_pow2 = (byte)Math.Log(denom, 2);
          Skip(1)();
          return new TimeSignature(deltaTime, num, denom_pow2, 24, 8);
        // Text
        case 8:
          var ttype = Byte();
          var txt = trackStrings[Short()];
          switch (ttype)
          {
            case 1:
              return new TextEvent(deltaTime, txt);
            case 3:
              trackName = txt;
              return new TrackName(deltaTime, txt);
            case 5:
              return new Lyric(deltaTime, txt);
            default:
              throw new NotImplementedException($"Text event {ttype} not implemented");
          }
        default:
          throw new NotImplementedException($"Message kind {kind} not yet known");
      }
    }
    private RBMid.TEMPO ReadTempo() => new RBMid.TEMPO
    {
      StartMillis = Float(),
      StartTick = UInt(),
      Tempo = Int()
    };
    private RBMid.TIMESIG ReadTimesig() => new RBMid.TIMESIG
    {
      Unknown = Int(),
      Tick = UInt(),
      Numerator = Short(),
      Denominator = Short()
    };
    private RBMid.BEAT ReadBeat() => new RBMid.BEAT
    {
      Tick = UInt(),
      Downbeat = Int() != 0
    };
  }
}
