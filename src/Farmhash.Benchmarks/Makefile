EXECUTABLE := benchmark-farmhash.exe
OBJS := farmhash.o benchmark-farmhash.o
CXXFLAGS := -std=c++11 -mavx -maes -O3 -I../../lib/benchmark/include
LINK_FLAGS := -L../../lib/benchmark/src -lpthread -lbenchmark -pthread
BENCH_DIR := ../../lib/benchmark
all: $(EXECUTABLE)

$(EXECUTABLE): $(OBJS) benchmark
				g++ -g -o $@ $(OBJS) $(LINK_FLAGS)

benchmark-farmhash.o: benchmark-farmhash.cpp

benchmark:
				cmake -DCMAKE_BUILD_TYPE=Release -DBENCHMARK_ENABLE_LTO=true $(BENCH_DIR)
				make -C $(BENCH_DIR)

farmhash.o: ../../lib/farmhash/src/farmhash.cc ../../lib/farmhash/src/farmhash.h
				g++ -c $(CXXFLAGS) $<

clean:
				rm -f $(OBJS) $(EXECUTABLE)
