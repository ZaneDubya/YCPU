//#define BENCHMARK

namespace YCPU.Hardware
{
    partial class YCPU
    {
#if BENCHMARK
        public void Benchmark(bool mmu_enabled, int count_runs)
        {
            long[] benchmark = new long[0x100];
            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
            System.Diagnostics.Stopwatch total = new System.Diagnostics.Stopwatch();
            total.Reset();
            total.Start();
            int count = 0;
            int cycles = 0;

            PS = 0x8000;
            PS_M = mmu_enabled;
            ushort ps = m_PS;

            for (int i = 0; i < 0x100; i++)
            {
                if (m_Opcodes[i].IsNOP)
                {
                    // Ignore NOPs for benchmarking.
                }
                else
                {
                    m_PS = ps;
                    for (int r = 0; r < 0x08; r++)
                        R[r] = 1;
                    for (int m = 0; m < 0x10000; m++)
                        SetMemory((ushort)m, 0x0001);
                    timer.Reset();
                    timer.Start();
                    for (int j = 0; j < count_runs; j++)
                    {
                        m_PS = ps;
                        ushort word = (ushort)(i & ((j & 0xFF) << 8));
                        m_Opcodes[i].Opcode(word, m_Opcodes[i].BitPattern);
                        count++;
                        cycles += m_Opcodes[i].Cycles;
                    }
                    timer.Stop();
                    benchmark[i] = timer.ElapsedTicks;
                }
            }
            total.Stop();

            long min_value = long.MaxValue;
            for (int i = 0; i < 0x100; i++)
            {
                if ((benchmark[i] != 0) && (benchmark[i] < min_value))
                    min_value = benchmark[i];
            }


            string[] lines = new string[0x101];
            for (int i = 0; i < 0x100; i++)
            {
                if (m_Opcodes[i].IsNOP)
                    lines[i] = "---";
                else
                    lines[i] = string.Format("{0}     {1:0.00}", m_Opcodes[i].Name, (float)benchmark[i] / (float)min_value);
            }
            lines[0x100] = string.Format("{0} opcodes, {1} cycles in {2} ms.", count, cycles, total.ElapsedMilliseconds);
            System.IO.File.WriteAllLines(string.Format("Benchmark{0}.txt", PS_M ? "M" : ""), lines);
        }
#endif
    }
}
